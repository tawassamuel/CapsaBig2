﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class CapsaDeskController : MonoBehaviour
{
    public enum PhaseDesk
    {
        Start = 0,
        Finish = 1,
        Close = 2
    }

    private const string StrSpade = "Spade";
    private const string StrHeart = "Heart";
    private const string StrClub = "Club";
    private const string StrDiamond = "Dm";

    private static CapsaDeskController singleton = null;
    public static CapsaDeskController Singleton
    {
        get
        {
            return singleton;
        }
    }

    [SerializeField] private bool autoStart = false;

    [Header("Game Rules"), SerializeField] private CapsaRuleData rulesData = null;
    public CapsaRuleData RulesData
    {
        get
        {
            return rulesData;
        }
    }
    [SerializeField] private TurnBaseManager turnManager = null;
    public TurnBaseManager TurnManager
    {
        get
        {
            return turnManager;
        }
    }

    [SerializeField] private CapsaBattleManager battleManager = null;
    public CapsaBattleManager BattleManager
    {
        get
        {
            return battleManager;
        }
    }
    [SerializeField] private AICapsaController aiController = null;
    [SerializeField] private LeanGameObjectPool poolCard = null;
    public LeanGameObjectPool ObjectPoolCard
    {
        get
        {
            return poolCard;
        }
    }
    [SerializeField] private AvatarMenuSelection avatarMenu = null;

    [SerializeField] private RectTransform centerBattle = null;
    [SerializeField] private List<CardData> currentCenterCards = new List<CardData>();
    [SerializeField] private List<CardData> availableDeck = new List<CardData>();
    [SerializeField] private List<PlayerEntity> players = new List<PlayerEntity>();
    
    private List<CardView> selectedPlayerCards = new List<CardView>();
    private List<CardView> spawnedCardCenter = new List<CardView>();
    private List<CardView> allRegisteredCards = new List<CardView>();

    private PlayerEntity Winner { get; set; }

#if UNITY_EDITOR
    [Header("Dev Tool (Editor Only)"), SerializeField] private GameObject panelDev = null;
    [SerializeField] private Button btnSpawnCard = null;
    [SerializeField] private Button btnCompareCard = null;
    [SerializeField] private Dropdown ddCardSpawn = null;
    [SerializeField] private CardData selectedCardToCenter = null;

    public CardData GetCardDataByName(string cardName)
    {
        return availableDeck.Where(x => x.GetCardName() == cardName).FirstOrDefault();
    }

    public bool CompareValidation()
    {
        return rulesData.GetWeightByCardData(currentCenterCards[0]) < rulesData.GetWeightByCardData(selectedPlayerCards[0].GetData());
    }
#endif

    public event System.Action<List<CardView>, CapsaRuleData.ComboOutput> OnAllSelectedCardByPlayer;

    #region Player-Listener-Controller
    private IEnumerator DelayAIAction(PlayerEntity currentAI)
    {
        yield return new WaitForSeconds(aiController.DelayMove);

        AICapsaController.GameState currentGameState = new AICapsaController.GameState();
        currentGameState.avaiableCards = currentAI.GetAvailableCards();
        currentGameState.currentPlayer = currentAI;
        CapsaRuleData.ComboOutput takeCombo = aiController.GenerateMove(currentGameState, this);

        yield return new WaitForEndOfFrame();

        if (takeCombo == null)
        {
            turnManager.GoNextTurn(players);
            yield break;
        }

        List<CardData> comboCards = takeCombo.availableCards;
        if (comboCards == null)
        {
            yield return new WaitForSeconds(aiController.DelayMove);

            turnManager.GoNextTurn(players);
            yield break;
        }

        currentAI.PullSomeCard(comboCards);
        battleManager.PutToTable(currentAI, comboCards, takeCombo.name);

        yield return new WaitForSeconds(aiController.DelayMove);

        turnManager.GoNextTurn(players);
    }
    public void OnSwitchedPlayer(PlayerEntity currentPlayer)
    {
        if (Winner != null)
            return;

        if (battleManager.GetLastSender() == currentPlayer)
        {
            Debug.Log("Winner battle is " + currentPlayer.GetName());
            battleManager.ClearAll(currentPlayer.GetName());
        }

        if (!currentPlayer.IsMine())
        {
            StartCoroutine(DelayAIAction(currentPlayer));
        }
    }
    public void ResetSelectedCardByPlayer()
    {
        for(int i = 0; i < selectedPlayerCards.Count; i++)
        {
            if (selectedPlayerCards[i] == null)
                continue;

            selectedPlayerCards[i].SetSelected(false);
        }

        selectedPlayerCards.Clear();
        OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, rulesData.GetAvailableCombo(turnManager.NumOfTurn(), ConvertToCardDataByView(selectedPlayerCards)));
    }
    public void SubmitSelectedCardByPlayer(CapsaRuleData.ComboOutput comboConfirm)
    {
        PlayerEntity myEntity = players.Where(x => x.IsMine() == true).FirstOrDefault();
        if (myEntity == null)
            return;

        List<CardData> cardDatums = new List<CardData>();
        for (int i = 0; i < selectedPlayerCards.Count; i++)
        {
            if (selectedPlayerCards[i] == null)
                continue;

            selectedPlayerCards[i].SetSelected(false);
            cardDatums.Add(selectedPlayerCards[i].GetData());
        }

        myEntity.PullSomeCard(cardDatums);
        battleManager.PutToTable(myEntity, cardDatums, comboConfirm.name);

        ResetSelectedCardByPlayer();
        turnManager.GoNextTurn(players);
    }
    public void OnToggleSelectedCardByPlayer(CardView selectedCard)
    {
        Debug.LogFormat("You Select {0} card", selectedCard.GetData().GetCardName());
        List<CardData> selectedCardsData = new List<CardData>();
        CapsaRuleData.ComboOutput availableCombo;
        if (selectedPlayerCards.Contains(selectedCard))
        {
            selectedCard.SetSelected(false);
            selectedPlayerCards.Remove(selectedCard);
            selectedCardsData = ConvertToCardDataByView(selectedPlayerCards);
            availableCombo = rulesData.GetAvailableCombo(turnManager.NumOfTurn(), selectedCardsData);

            if (!battleManager.IsTableContainCards())
                OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, availableCombo);
            else
            {
                if (availableCombo == null)
                {
                    OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, null);
                    return;
                }
                CapsaRuleData.ComboOutput tableCombo = battleManager.HasKindCombo(turnManager.NumOfTurn(), rulesData);
                bool canBeat = rulesData.CanBeatCardOnTable(availableCombo, tableCombo);
                if (!canBeat)
                    OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, null);
                else
                    OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, availableCombo);
            }
            return;
        }

        selectedCard.SetSelected(true);
        selectedPlayerCards.Add(selectedCard);
        selectedCardsData = ConvertToCardDataByView(selectedPlayerCards);
        availableCombo = rulesData.GetAvailableCombo(turnManager.NumOfTurn(), selectedCardsData);

        if (!battleManager.IsTableContainCards())
            OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, rulesData.GetAvailableCombo(turnManager.NumOfTurn(), ConvertToCardDataByView(selectedPlayerCards)));
        else
        {
            if (availableCombo == null)
            {
                OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, null);
                return;
            }
            CapsaRuleData.ComboOutput tableCombo = battleManager.HasKindCombo(turnManager.NumOfTurn(), this.rulesData);
            bool canBeat = rulesData.CanBeatCardOnTable(availableCombo, tableCombo);
            if (!canBeat)
                OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, null);
            else
                OnAllSelectedCardByPlayer?.Invoke(selectedPlayerCards, availableCombo);
        }
    }
    private void AddedCardOnPlayer(PlayerEntity entity, CardData card, RectTransform ownContainer)
    {
        Debug.Log("Update cards for " + entity.GetName());

        GameObject spawned = poolCard.Spawn(ownContainer);
        CardView cardView = spawned.GetComponent<CardView>();
        if (entity.IsMine())
            cardView.InitializeView(card, OnToggleSelectedCardByPlayer);
        else
            cardView.InitializeView(card);
        cardView.SetShowCard(entity.IsMine());
        allRegisteredCards.Add(cardView);
    }
    private void RemovedCardsFromPlayer(PlayerEntity entity, List<CardData> cards, RectTransform ownContainer)
    {
        foreach(CardData card in cards)
        {
            if (card == null)
                continue;

            CardView cardView = allRegisteredCards.Where(x => x.GetData() == card).FirstOrDefault();
            if (cardView != null)
            {
                poolCard.Despawn(cardView.gameObject);
            }
        }

        List<PlayerEntity> others = new List<PlayerEntity>(players.Where(x => x != entity).ToList());

        if (entity.GetAvailableCards().Count <= 0)
        {
            Winner = entity;
            entity.SetAvatarState(AvatarData.State.Happy);
            InvokePhase(PhaseDesk.Finish);

            foreach (PlayerEntity other in others)
            {
                other.SetAvatarState(AvatarData.State.Sad);
            }
        } else
        {
            int myCardNum = entity.GetAvailableCards().Count;
            
            others = others.OrderBy(x => x.GetAvailableCards().Count).ToList();
            foreach (PlayerEntity other in others)
            {
                int otherCardNum = other.GetAvailableCards().Count;
                if (otherCardNum <= 5 && Mathf.Abs(myCardNum - otherCardNum) >= 6)
                {
                    entity.SetAvatarState(AvatarData.State.Sad);
                    if (entity.IsMine())
                    {
                        AudioManager.Singleton.PlayMusicByName("Losing");
                    }
                }
                else if (otherCardNum > 5 && Mathf.Abs(myCardNum - otherCardNum) >= 6)
                {
                    entity.SetAvatarState(AvatarData.State.Happy);
                    if (entity.IsMine())
                    {
                        AudioManager.Singleton.PlayMusicByName("Winning");
                    }
                    other.SetAvatarState(AvatarData.State.Sad);
                    if (other.IsMine())
                    {
                        AudioManager.Singleton.PlayMusicByName("Losing");
                    }
                }
                else
                {
                    entity.SetAvatarState(AvatarData.State.Idle);
                    if (entity.IsMine())
                        AudioManager.Singleton.PlayMusicByName("GameOn");
                }
            }
        }
    }
    private void ShuffleDeck()
    {
        for (int i = availableDeck.Count - 1; i > 0; i--)
        {
            int index = Random.Range(0, i + 1);

            CardData cardDeck = availableDeck[index];
            availableDeck[index] = availableDeck[i];
            availableDeck[i] = cardDeck;
        }
    }
    private void InitializeAllPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetContainer() == null)
                continue;

            players[i].Initialize(AddedCardOnPlayer, null, RemovedCardsFromPlayer);
        }
    }
    private void ClearAllPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetContainer() == null)
                continue;

            players[i].Clear(AddedCardOnPlayer, null, RemovedCardsFromPlayer);
        }
    }
    private void ShareAllCardsToPlayers()
    {
        int indexPlayer = 0;

        ClearAllRegisteredCards();
#if UNITY_EDITOR
        ddCardSpawn.ClearOptions();
        List<Dropdown.OptionData> listOtherCards = new List<Dropdown.OptionData>();
#endif
        for (int i = 0; i < availableDeck.Count; i++)
        {
            if (availableDeck[i] == null)
                continue;

            CardData availableCard = availableDeck[i];
            players[indexPlayer].AddCard(availableCard);
#if UNITY_EDITOR
            if (!players[indexPlayer].IsMine())
            {
                Dropdown.OptionData optionData = new Dropdown.OptionData();
                optionData.text = availableCard.GetCardName();
                listOtherCards.Add(optionData);
            }
#endif
            indexPlayer++;
            if (indexPlayer >= players.Count)
                indexPlayer = 0;

            //availableDeck[i] = null;
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null)
                continue;

            bool doReshuffling = rulesData.ReshufflingByCards(players[i].GetAvailableCards());
            if (doReshuffling)
            {
                StartGame();
                Debug.Log("Reshuffling...");
                return;
            }
        }

        //availableDeck.Clear();
#if UNITY_EDITOR
        listOtherCards = listOtherCards.OrderBy(x => x.text).ToList();
        Dropdown.OptionData vacant = new Dropdown.OptionData();
        vacant.text = "None";
        listOtherCards.Insert(0, vacant);
        ddCardSpawn.AddOptions(listOtherCards);
        ddCardSpawn.onValueChanged.RemoveAllListeners();
        ddCardSpawn.onValueChanged.AddListener((intCalled) => {
            List<Dropdown.OptionData> listFetch = ddCardSpawn.options;
            Dropdown.OptionData selectOn = listFetch[intCalled];
            selectedCardToCenter = GetCardDataByName(selectOn.text);
        });

        btnSpawnCard.onClick.RemoveAllListeners();
        btnSpawnCard.onClick.AddListener(() => {
            if (selectedCardToCenter == null)
                return;

            foreach (CardView cardView in spawnedCardCenter)
            {
                poolCard.Despawn(cardView.gameObject);
            }
            currentCenterCards.Clear();
            spawnedCardCenter.Clear();
            RenderCardBattleCenter(new List<CardData> { selectedCardToCenter });
        });

        btnCompareCard.onClick.RemoveAllListeners();
        btnCompareCard.onClick.AddListener(() => {
            if (selectedPlayerCards.Count <= 0)
                return;

            if (selectedPlayerCards.Count > 1)
            {
                Debug.Log("Dev tool only support for single pattern");
                return;
            }

            if (currentCenterCards.Count <= 0)
                return;

            if (CompareValidation())
            {
                Debug.Log("Card valid and you win the battle");
            }
            else
            {
                Debug.Log("Card invalid");
            }
        });
#endif
    }
    private void OnSelectedAvatarByPlayer(AvatarData dataAvatar)
    {
        PlayerEntity getMine = players.Where(x => x.IsMine() == true).FirstOrDefault();
        if (getMine == null)
            return;

        getMine.SetAvatarData(dataAvatar);

        List<AvatarData> otherData = new List<AvatarData>();
        otherData.AddRange(Resources.LoadAll<AvatarData>("Others/Avatar").ToList());
        otherData = otherData.Where(x => x != dataAvatar).ToList();
        List<PlayerEntity> getOthers = players.Where(x => x.IsMine() == false).ToList();

        int index = 0;
        foreach (PlayerEntity entity in getOthers)
        {
            if (entity == null)
                continue;

            entity.SetAvatarData(otherData[index]);
            index++;
            if (index >= otherData.Count)
                index = 0;
        }

        StartGame();
    }
    #endregion

    #region Data-Management
    public static List<CardData> ConvertToCardDataByView(List<CardView> input)
    {
        List<CardData> result = new List<CardData>();

        foreach (CardView card in input)
        {
            if (card == null)
                continue;

            if (card.GetData() == null)
                continue;

            result.Add(card.GetData());
        }

        return result;
    }
    public void ClearAllRegisteredCards()
    {
        for (int i = 0; i < allRegisteredCards.Count; i++)
        {
            if (allRegisteredCards[i] == null)
                continue;

            allRegisteredCards[i].SetSelected(false);
            allRegisteredCards[i].SetShowCard(false);

            poolCard.Despawn(allRegisteredCards[i].gameObject);
        }

        allRegisteredCards.Clear();
    }
    #endregion

    #region Gameplay
    public void PassAction()
    {
        turnManager.GoNextTurn(players);
    }
    public void RenderCardBattleCenter(List<CardData> putCard)
    {
        currentCenterCards.AddRange(putCard);
        for (int i = 0; i < putCard.Count; i++)
        {
            GameObject spawned = poolCard.Spawn(centerBattle);
            CardView cardView = spawned.GetComponent<CardView>();
            cardView.InitializeView(putCard[i]);
            cardView.SetShowCard(true);
            spawnedCardCenter.Add(cardView);
        }
    }
    public bool IsMyTurn()
    {
        PlayerEntity myEntity = players.Where(x => x.IsMine() == true).FirstOrDefault();
        return turnManager.WhoCurrentTurn() == myEntity;
    }
    public void StartGame()
    {
        InvokePhase(PhaseDesk.Start);
    }
    void InvokePhase(PhaseDesk phase)
    {
        switch (phase)
        {
            case PhaseDesk.Start:
                ClearAllPlayers();

                ShuffleDeck();
                InitializeAllPlayers();
                ShareAllCardsToPlayers();
                Winner = null;
                battleManager.ClearAll();
                turnManager.Initialize();
                turnManager.SwitchTurn(turnManager.WhoGotFirstTurn(players));
                battleManager.SetWinnerAnnounce("", false);

                AudioManager.Singleton.PlayMusicByName("GameOn");
                break;
            case PhaseDesk.Finish:
                if (Winner.IsMine())
                {
                    battleManager.SetWinnerAnnounce("YOU WIN", true);
                } else
                {
                    battleManager.SetWinnerAnnounce("YOU LOSE", true);
                }
                break;
            case PhaseDesk.Close:
                ClearAllPlayers();
                battleManager.ClearAll();
                turnManager.Initialize();
                Winner = null;
                battleManager.SetWinnerAnnounce("", false);

                AudioManager.Singleton.PlayMusicByName("MainMenu");
                break;
        }
    }
    #endregion

    private void Awake()
    {
        singleton = this;

        if (turnManager == null)
            turnManager = GetComponent<TurnBaseManager>();

        if (battleManager == null)
            battleManager = GetComponent<CapsaBattleManager>();

        if (aiController == null)
            aiController = GetComponentInChildren<AICapsaController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        availableDeck.AddRange(Resources.LoadAll<CardData>(StrSpade).ToList());
        availableDeck.AddRange(Resources.LoadAll<CardData>(StrHeart).ToList());
        availableDeck.AddRange(Resources.LoadAll<CardData>(StrClub).ToList());
        availableDeck.AddRange(Resources.LoadAll<CardData>(StrDiamond).ToList());

        if (turnManager != null)
        {
            turnManager.OnCatchPlayerToPlay += OnSwitchedPlayer;
        }

        if (avatarMenu == null)
        {
            avatarMenu = AvatarMenuSelection.Singleton;
            avatarMenu.OnSelectedAvatarByPlayer += OnSelectedAvatarByPlayer;
        }

        if (autoStart)
            InvokePhase(PhaseDesk.Start);
        else
            InvokePhase(PhaseDesk.Close);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                InvokePhase(PhaseDesk.Start);
            }
        }
    }
#endif
}
