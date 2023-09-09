using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "CapsaBig2/Card Data", order = 0)]
public class CardData : ScriptableObject
{
    public enum Rank
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Joker = 14
    }

    public enum TypeSymbol
    {
        Spade = 1,
        Heart = 2,
        Club = 3,
        Diamond = 4
    }

    [SerializeField] private TypeSymbol type = TypeSymbol.Spade;
    [SerializeField] private Rank rank = Rank.Ace;
    [SerializeField] private Texture2D texture = null;

    public Rank GetRank()
    {
        return rank;
    }

    public TypeSymbol GetSuit()
    {
        return type;
    }

    public string GetCardName()
    {
        return type.ToString() + " " + rank.ToString();
    }

    public Texture2D GetTexture2D()
    {
        return texture;
    }
}
