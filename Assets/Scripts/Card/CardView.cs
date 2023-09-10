using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
public class CardView : MonoBehaviour, IPoolable
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

    public void OnSpawn(){}

    public void OnDespawn()
    {
        cardData = null;
        imageRenderer.texture = null;
        imageRenderer.gameObject.SetActive(false);
        gameObject.name = "Card";

        selected = false;
        transform.localScale = Vector3.one * 1f;

        btnOnClick.onClick.RemoveAllListeners();
    }
}
