using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private bool showAtStar = false;

    [SerializeField] private CardData cardData = null;
    [SerializeField] private RawImage imageRenderer = null;
    [SerializeField] private Button btnOnClick = null;

    private bool selected = false;

    public void SetShowCard(bool paramArg)
    {
        imageRenderer.gameObject.SetActive(paramArg);
        btnOnClick.interactable = paramArg;
    }

    public void SetSelected(bool paramArg)
    {
        selected = paramArg;
        transform.localScale = Vector3.one * (selected ? 1.25f : 1f);
    }

    public void InitializeView()
    {
        imageRenderer.texture = cardData.GetTexture2D();
        gameObject.name = cardData.GetCardName();
    }

    public void InitializeView(CardData inputData, System.Action<CardView> onClick = null)
    {
        cardData = inputData;
        imageRenderer.texture = cardData.GetTexture2D();
        gameObject.name = cardData.GetCardName();

        btnOnClick.onClick.RemoveAllListeners();
        btnOnClick.onClick.AddListener(() => onClick?.Invoke(this));
    }

    public CardData GetData()
    {
        return cardData;
    }
}
