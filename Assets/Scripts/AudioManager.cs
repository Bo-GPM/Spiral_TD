using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]public AudioSource[] AudioSources;
    [HideInInspector] static public AudioManager audioInstance;

    private void Awake()
    {
        audioInstance = this;
    }
    public void PlayAudio(int index)
    {
        //get index and play audio
      AudioSources[index].Play();
    }

}

