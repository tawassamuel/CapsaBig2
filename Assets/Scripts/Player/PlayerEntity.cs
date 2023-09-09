using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerEntity
{
    [SerializeField] private string name;
    [SerializeField] private bool isMine = false;
    [SerializeField] private RectTransform container;
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
    }

    public void Clear(System.Action<PlayerEntity, CardData, RectTransform> listenerAdd,
        System.Action<PlayerEntity, CardData, RectTransform> listenerRemove,
        System.Action<PlayerEntity, List<CardData>, RectTransform> listenerRemoveAny)
    {
        cards = new List<CardData>();
        OnAddedPlayerCard -= listenerAdd;
        OnRemovedPlayerCard -= listenerRemove;
        OnRemovedPlayerCards += listenerRemoveAny;
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

        return cards.Count;
    }

    public bool PullCard(CardData card)
    {
        if (cards == null)
            return false;

        bool result = cards.Remove(card);
        OnRemovedPlayerCard?.Invoke(this, card, container);

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
}
