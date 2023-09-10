using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarDataView : MonoBehaviour
{
    [SerializeField] private AvatarData data = null;
    [SerializeField] private Image img = null;
    [SerializeField] private Button btn = null;

    private void Start()
    {
        img.sprite = data.GetEmoteByState(AvatarData.State.Idle).sprite;

        btn.onClick.AddListener(() => AvatarMenuSelection.Singleton.OnSelectAvatarByPlayer(data));
    }
}
