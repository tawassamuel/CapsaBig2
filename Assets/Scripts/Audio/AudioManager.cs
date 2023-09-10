using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager singleton = null;
    public static AudioManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    [SerializeField] private List<AudioClipData> allData = new List<AudioClipData>();
    [SerializeField] private AudioSource sourceMusic = null;

    private AudioClipData currentMusic = null;

    public void PlayMusicByName(string name)
    {
        if (allData == null)
            return;

        if (allData.Count <= 0)
            return;

        AudioClipData getClipData = allData.Where(x => x.GetName() == name).FirstOrDefault();
        if (getClipData != null)
        {
            if (currentMusic != null)
            {
                if (currentMusic.GetName() == getClipData.GetName())
                    return;
            }

            if (sourceMusic.isPlaying)
                sourceMusic.Stop();

            currentMusic = getClipData;
            sourceMusic.clip = getClipData.GetClip();
            sourceMusic.loop = true;
            sourceMusic.Play();
        }
    }
    
    private void Awake()
    {
        singleton = this;

        allData = new List<AudioClipData>();
        allData.AddRange(Resources.LoadAll<AudioClipData>("Audio"));
    }
}
