using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapsaBattleManager : MonoBehaviour
{
    [SerializeField] private Image imgCurrentDealer = null;
    [SerializeField] private TMP_Text textCurrentCombo = null;
    [SerializeField] private RectTransform container = null;
    [SerializeField] private List<CardView> currentTable = null;
    [SerializeField] private GameObject announceWinnerPanel = null;
    [SerializeField] private TMP_Text textAnnounceWinner = null;
    public List<CardView> CurrentTable
    {
        get
        {
            return currentTable;
        }
    }

    private PlayerEntity lastSender = null;
    public PlayerEntity GetLastSender()
    {
        return lastSender;
    }

    public bool IsTableContainCards()
    {
        if (currentTable == null)
            return false;

        return currentTable.Count > 0;
    }

    public void SetWinnerAnnounce(string textInput, bool active)
    {
        announceWinnerPanel.SetActive(active);
        textAnnounceWinner.text = textInput;
    }

    public CapsaRuleData.ComboOutput HasKindCombo(int numOfTurn, CapsaRuleData ruleImplement)
    {
        if (currentTable == null)
            return null;

        if (currentTable.Count <= 0)
            return null;

        List<CardData> cards = CapsaDeskController.ConvertToCardDataByView(currentTable);
        return ruleImplement.GetAvailableCombo(numOfTurn, cards);
    }

    public void ClearAll(string winnerDealer = "")
    {
        if (!string.IsNullOrEmpty(winnerDealer))
            textCurrentCombo.text = winnerDealer + " won the battle. Put some cards";

        textCurrentCombo.text = "";
        imgCurrentDealer.gameObject.SetActive(false);

        foreach (CardView card in currentTable)
        {
            if (card == null)
                continue;

            //Destroy(card.gameObject);
            CapsaDeskController.Singleton.ObjectPoolCard.Despawn(card.gameObject);
        }
        currentTable.Clear();
    }

    public void PutToTable(PlayerEntity senderEntity, List<CardData> input, string nameCombo, GameObject prefab)
    {
        if (currentTable == null)
            currentTable = new List<CardView>();

        lastSender = senderEntity;
        textCurrentCombo.text = nameCombo;
        imgCurrentDealer.gameObject.SetActive(true);
        imgCurrentDealer.sprite = senderEntity.GetProfile();

        foreach (CardView card in currentTable)
        {
            if (card == null)
                continue;

            CapsaDeskController.Singleton.ObjectPoolCard.Despawn(card.gameObject);
            //Destroy(card.gameObject);
        }
        currentTable.Clear();

        for (int i = 0; i < input.Count; i++)
        {
            GameObject spawned = CapsaDeskController.Singleton.ObjectPoolCard.Spawn(container);
            CardView cardView = spawned.GetComponent<CardView>();
            cardView.InitializeView(input[i]);
            cardView.SetShowCard(true);
            currentTable.Add(cardView);
        }
    }
}
