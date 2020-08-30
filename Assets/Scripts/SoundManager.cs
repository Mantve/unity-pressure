using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private AudioSource ambientSource;
    [SerializeField]
    private AudioSource backgroundSource;

    [SerializeField]
    private AudioClip buttonClick;

    [SerializeField]
    private Slider ambientSourceSlider;
    [SerializeField]
    private Slider backgroundSourceSlider;

    private const string AmbientVolume = "AmbientVolume";
    private const string BackgroundVolume = "BackgroundVolume";

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
    private void Start()
    {
        if (PlayerPrefs.HasKey(AmbientVolume))
        {
            ambientSourceSlider.value = PlayerPrefs.GetFloat(AmbientVolume);
        }
        if (PlayerPrefs.HasKey(BackgroundVolume))
        {
            backgroundSourceSlider.value = PlayerPrefs.GetFloat(BackgroundVolume);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        ambientSource.PlayOneShot(clip);
    }
    public void PlayButtonClick()
    {
        ambientSource.PlayOneShot(buttonClick);
    }
    public void OnAmbientSliderUpdate()
    {
        ambientSource.volume = ambientSourceSlider.value;
        PlayerPrefs.SetFloat(AmbientVolume, ambientSource.volume);
        PlayerPrefs.Save();
    }
    public void OnBacgroundSliderUpdate()
    {
        backgroundSource.volume = backgroundSourceSlider.value;
        PlayerPrefs.SetFloat(BackgroundVolume, backgroundSource.volume);
        PlayerPrefs.Save();
    }
}
