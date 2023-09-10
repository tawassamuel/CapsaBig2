using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarMenuSelection : MonoBehaviour
{
    private static AvatarMenuSelection singleton = null;
    public static AvatarMenuSelection Singleton
    {
        get
        {
            return singleton;
        }
    }

    private AvatarData player = null;
    public AvatarData GetPlayer()
    {
        return player;
    }

    [SerializeField] private GameObject window = null;

    private void Awake()
    {
        singleton = this;
    }

    public event System.Action<AvatarData> OnSelectedAvatarByPlayer;

    public void OnSelectAvatarByPlayer(AvatarData data) {
        player = data;
        OnSelectedAvatarByPlayer?.Invoke(data);

        window.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        window.SetActive(true);
    }
}
