using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICapsaController : MonoBehaviour
{
    [System.Serializable]
    public class GameState
    {
        public List<CardData> avaiableCards = new List<CardData>();
        public PlayerEntity currentPlayer;

        public GameState()
        {
            avaiableCards = new List<CardData>();
            currentPlayer = null;
        }

        public GameState(List<CardData> myCards, PlayerEntity currentPlayerIndex)
        {
            avaiableCards = myCards;
            currentPlayer = currentPlayerIndex;
        }

        public PlayerEntity GetCurrentPlayer()
        {
            return currentPlayer;
        }
    }

    [SerializeField] private float delayMove = 0.5f;
    public float DelayMove
    {
        get
        {
            return delayMove;
        }
    }

    private List<CapsaRuleData.ComboOutput> GeneratePossibleLegalComboOutput(GameState gameState, CapsaDeskController deskController)
    {
        //List<CardData> currentHand = gameState.GetCurrentPlayer().GetAvailableCards();
        List<CapsaRuleData.ComboOutput> legalCombo = new List<CapsaRuleData.ComboOutput>();
        CapsaBattleManager battleManager = deskController.BattleManager;
        TurnBaseManager turnManager = deskController.TurnManager;

        List<CapsaRuleData.ComboOutput> allLegalCombo = deskController.RulesData.GetAllAvailableCombo(turnManager.NumOfTurn(), gameState.avaiableCards);

        if (!battleManager.IsTableContainCards())
            return allLegalCombo;
        else
        {
            CapsaRuleData.ComboOutput tableCombo = battleManager.HasKindCombo(turnManager.NumOfTurn(), deskController.RulesData);
            if (tableCombo == null)
            {
                return allLegalCombo;
            }

            foreach (CapsaRuleData.ComboOutput combo in allLegalCombo)
            {
                bool canBeat = deskController.RulesData.CanBeatCardOnTable(combo, tableCombo);
                if (!canBeat)
                    continue;
                else
                    legalCombo.Add(combo);
            }
        }

        return legalCombo;
    }

    private CapsaRuleData.ComboOutput ChooseBestCombo(List<CapsaRuleData.ComboOutput> availableCombos)
    {
        if (availableCombos == null)
            return null;

        if (availableCombos.Count <= 0)
            return null;

        return availableCombos.OrderByDescending(x => x.basedValue).FirstOrDefault();
    }

    public CapsaRuleData.ComboOutput GenerateMove(GameState gameState, CapsaDeskController deskController)
    {
        return ChooseBestCombo(GeneratePossibleLegalComboOutput(gameState, deskController));
    }
}
