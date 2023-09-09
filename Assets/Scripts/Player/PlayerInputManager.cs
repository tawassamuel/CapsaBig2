using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{
    private CapsaDeskController deskController = null;

    [SerializeField] private GameObject playerSelectedMenu = null;
    [SerializeField] private Button btnSelectedReset = null;
    [SerializeField] private Button btnSelectedSubmit = null;

    public void OnCurrentSelectedCards(List<CardView> selectedCards, CapsaRuleData.ComboOutput combo)
    {
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

                deskController.SubmitSelectedCardByPlayer();
            });
        }
    }
}
