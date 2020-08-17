using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private AudioSource mainSource;
    [SerializeField]
    private AudioSource backgroundMusic;

    [SerializeField]
    private AudioClip buttonClick;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        mainSource.PlayOneShot(clip);
    }
    public void PlayButtonClick()
    {
        mainSource.PlayOneShot(buttonClick);
    }
}
