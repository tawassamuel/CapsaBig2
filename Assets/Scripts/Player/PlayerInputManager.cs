using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{
    private CapsaDeskController deskController = null;
    private TurnBaseManager turnManager = null;

    [SerializeField] private GameObject playerSelectedMenu = null;
    [SerializeField] private Button btnSelectedReset = null;
    [SerializeField] private Button btnSelectedSubmit = null;
    [SerializeField] private Button btnPass = null;

    private CapsaRuleData.ComboOutput lastConfirmCombo = null;

    public void OnSwitchedPlayer(PlayerEntity currentPlayer)
    {
        if (currentPlayer.IsMine())
        {
            if (turnManager.NumOfTurn() <= 1)
            {
                btnPass.interactable = false;
                btnPass.gameObject.SetActive(false);
                return;
            }else if (deskController.BattleManager.CurrentTable.Count <= 0)
            {
                btnPass.interactable = false;
                btnPass.gameObject.SetActive(false);
                return;
            }
            btnPass.interactable = true;
            btnPass.gameObject.SetActive(true);
        } else
        {
            btnPass.interactable = false;
            btnPass.gameObject.SetActive(false);
        }
    }

    public void OnCurrentSelectedCards(List<CardView> selectedCards, CapsaRuleData.ComboOutput combo)
    {
        lastConfirmCombo = combo;
        btnSelectedSubmit.interactable = combo != null;
        if (combo != null)
            Debug.Log("Available combo is " + combo.name);

        if (selectedCards.Count <= 0)
        {
            playerSelectedMenu.SetActive(false);
            return;
        }

        playerSelectedMenu.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        deskController = CapsaDeskController.Singleton;
        turnManager = deskController.TurnManager;
        if (turnManager != null)
        {
            turnManager.OnCatchPlayerToPlay += OnSwitchedPlayer;
        }

        if (deskController != null)
        {
            deskController.OnAllSelectedCardByPlayer += OnCurrentSelectedCards;
            playerSelectedMenu.SetActive(false);

            btnSelectedReset.onClick.AddListener(deskController.ResetSelectedCardByPlayer);
            btnSelectedSubmit.onClick.AddListener(() => {
                bool isMyTurn = deskController.IsMyTurn();

                if (!isMyTurn)
                {
                    Debug.Log("Please wait, this is not your turn");
                    return;
                }

                deskController.SubmitSelectedCardByPlayer(lastConfirmCombo);
            });

            btnPass.onClick.AddListener(() =>
            {
                bool isMyTurn = deskController.IsMyTurn();

                if (!isMyTurn)
                {
                    Debug.Log("Please wait, this is not your turn");
                    return;
                }

                deskController.PassAction();
            });
        }
    }
}
