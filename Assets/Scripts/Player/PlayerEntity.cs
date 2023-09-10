using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class PlayerEntity
{
    [SerializeField] private string name;
    [SerializeField] private bool isMine = false;
    [SerializeField] private RectTransform container;
    [SerializeField] private AvatarData selectedData;
    [SerializeField] private Image imgAvatar = null;
    [SerializeField] private TMP_Text txtCards = null;
    [SerializeField] private List<CardData> cards;

    public event System.Action<PlayerEntity, CardData, RectTransform> OnAddedPlayerCard;
    public event System.Action<PlayerEntity, CardData, RectTransform> OnRemovedPlayerCard;
    public event System.Action<PlayerEntity, List<CardData>, RectTransform> OnRemovedPlayerCards;

    public PlayerEntity()
    {
        name = "";
        cards = new List<CardData>();
        container = null;
        OnAddedPlayerCard = new System.Action<PlayerEntity, CardData, RectTransform>((paramEntity, paramCard, paramContainer) => { });
        OnRemovedPlayerCard = new System.Action<PlayerEntity, CardData, RectTransform>((paramEntity, paramCard, paramContainer) => { });
        OnRemovedPlayerCards = new System.Action<PlayerEntity, List<CardData>, RectTransform>((paramEntity, paramCards, paramContainer) => { });
    }

    public PlayerEntity(List<CardData> initializeCards)
    {
        name = "";
        cards = initializeCards;
        container = null;
        OnAddedPlayerCard = new System.Action<PlayerEntity, CardData, RectTransform>((paramEntity, paramCard, paramContainer) => {});
        OnRemovedPlayerCard = new System.Action<PlayerEntity, CardData, RectTransform>((paramEntity, paramCard, paramContainer) => { });
        OnRemovedPlayerCards = new System.Action<PlayerEntity, List<CardData>, RectTransform>((paramEntity, paramCards, paramContainer) => { });
    }

    public void Initialize(System.Action<PlayerEntity, CardData, RectTransform> listenerAdd, 
        System.Action<PlayerEntity, CardData, RectTransform> listenerRemove,
        System.Action<PlayerEntity, List<CardData>, RectTransform> listenerRemoveAny)
    {
        cards = new List<CardData>();
        OnAddedPlayerCard += listenerAdd;
        OnRemovedPlayerCard += listenerRemove;
        OnRemovedPlayerCards += listenerRemoveAny;
        SetAvatarState(AvatarData.State.Idle);
    }

    public void Clear(System.Action<PlayerEntity, CardData, RectTransform> listenerAdd,
        System.Action<PlayerEntity, CardData, RectTransform> listenerRemove,
        System.Action<PlayerEntity, List<CardData>, RectTransform> listenerRemoveAny)
    {
        cards = new List<CardData>();
        OnAddedPlayerCard -= listenerAdd;
        OnRemovedPlayerCard -= listenerRemove;
        OnRemovedPlayerCards += listenerRemoveAny;
        if (txtCards != null)
            txtCards.text = cards.Count.ToString();
    }

    public bool HasCard(CardData.Rank inputRank, CardData.TypeSymbol inputSuit)
    {
        foreach (CardData card in cards)
        {
            if (card == null)
                continue;

            if (card.GetRank() == inputRank && card.GetSuit() == inputSuit)
                return true;
        }

        return false;
    }

    public bool HasCard(CardData card)
    {
        if (cards == null)
            return false;

        return cards.Contains(card);
    }

    public int AddCard(CardData card)
    {
        if (cards == null)
            return 0;

        cards.Add(card);
        OnAddedPlayerCard?.Invoke(this, card, container);

        if (txtCards != null)
            txtCards.text = cards.Count.ToString();

        return cards.Count;
    }

    public bool PullCard(CardData card)
    {
        if (cards == null)
            return false;

        bool result = cards.Remove(card);
        OnRemovedPlayerCard?.Invoke(this, card, container);
        if (txtCards != null)
            txtCards.text = cards.Count.ToString();

        return result;
    }

    public bool PullSomeCard(List<CardData> cards)
    {
        if (this.cards == null)
            return false;

        foreach(CardData card in cards)
        {
            if (card == null)
                continue;

            bool result = this.cards.Remove(card);
            if (!result)
                return false;
        }

        OnRemovedPlayerCards?.Invoke(this, cards, container);
        if (txtCards != null)
            txtCards.text = this.cards.Count.ToString();

        return true;
    }

    public List<CardData> GetAvailableCards()
    {
        return cards;
    }

    public void UpdateName(string input)
    {
        name = input;
    }

    public string GetName()
    {
        return name;
    }

    public bool IsMine()
    {
        return isMine;
    }

    public RectTransform GetContainer()
    {
        return container;
    }

    public void SetAvatarData(AvatarData input)
    {
        selectedData = input;
        imgAvatar.sprite = selectedData.GetEmoteByState(AvatarData.State.Idle).sprite;
    }

    public void SetAvatarState(AvatarData.State stateInput)
    {
        if (selectedData == null)
            return;

        imgAvatar.sprite = selectedData.GetEmoteByState(stateInput).sprite;
    }

    public Sprite GetProfile()
    {
        return imgAvatar.sprite;
    }
}
