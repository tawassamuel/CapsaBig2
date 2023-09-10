using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Clip Data", menuName = "CapsaBig2/Audio Clip Data", order = 0)]
public class AudioClipData : ScriptableObject
{
    [SerializeField] private string clipName = "Default";
    [SerializeField] private AudioClip clip = null;

    public string GetName()
    {
        return clipName;
    }

    public AudioClip GetClip()
    {
        return clip;
    }
}
