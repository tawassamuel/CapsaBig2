using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnBaseManager : MonoBehaviour
{
    [SerializeField] TMP_Text txtCurrentTurn = null;
    [SerializeField] TMP_Text txtNumTurn = null;
    private int numTurn = 0;
    public int NumOfTurn()
    {
        return numTurn;
    }

    private PlayerEntity currentPlayer = null;

    public event System.Action<PlayerEntity> OnCatchPlayerToPlay;

    public void Initialize()
    {
        numTurn = 0;
        txtNumTurn.text = "Turn " + numTurn;
    }

    public void SwitchTurn(PlayerEntity input)
    {
        numTurn++;
        currentPlayer = input;
        txtCurrentTurn.text = (!currentPlayer.IsMine()) ? string.Format("{0} Turn", currentPlayer.GetName()) : "Your Turn";
        txtNumTurn.text = "Turn " + numTurn;

        OnCatchPlayerToPlay?.Invoke(currentPlayer);
    }

    public void GoNextTurn(List<PlayerEntity> players)
    {
        int indexCurrent = players.IndexOf(currentPlayer);
        int indexNext = indexCurrent + 1;
        if (indexNext >= players.Count)
            indexNext = 0;

        SwitchTurn(players[indexNext]);
    }

    public PlayerEntity WhoGotFirstTurn(List<PlayerEntity> allPlayers)
    {
        foreach(PlayerEntity player in allPlayers)
        {
            if (player.HasCard(CardData.Rank.Three, CardData.TypeSymbol.Diamond))
                return player;
        }

        return allPlayers[Random.Range(0, allPlayers.Count)];
    }

    public PlayerEntity WhoCurrentTurn()
    {
        return currentPlayer;
    }
}
