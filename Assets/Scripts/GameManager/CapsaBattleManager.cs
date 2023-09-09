using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsaBattleManager : MonoBehaviour
{
    [SerializeField] private RectTransform container = null;
    [SerializeField] private List<CardView> currentTable = null;

    public bool IsTableContainCards()
    {
        if (currentTable == null)
            return false;

        return currentTable.Count > 0;
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

    public void PutToTable(List<CardData> input, GameObject prefab)
    {
        if (currentTable == null)
            currentTable = new List<CardView>();

        foreach (CardView card in currentTable)
        {
            if (card == null)
                continue;

            Destroy(card.gameObject);
        }
        currentTable.Clear();

        for (int i = 0; i < input.Count; i++)
        {
            GameObject spawned = Instantiate(prefab, container);
            CardView cardView = spawned.GetComponent<CardView>();
            cardView.InitializeView(input[i]);
            cardView.SetShowCard(true);
            currentTable.Add(cardView);
        }
    }
}
