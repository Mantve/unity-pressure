using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private bool isFadingOut = false;
    private bool isFadingIn = false;
    [SerializeField]
    private float fadeInLerpSpeed = 0.1f;
    [SerializeField]
    private float fadeOutLerpSpeed = 1.5f;

    [SerializeField]
    private Image loadingScreenImage;

    private string waitingToLoadLevel;

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
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    private void LoadWaitingLevel()
    {
        LoadLevel(waitingToLoadLevel);
    }
    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(WaitUntilFinishFading(FadeOut));
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    public void LoadOnFullyFadded(string levelName)
    {
        waitingToLoadLevel = levelName;
        StartCoroutine(Fade(fadeInLerpSpeed, true));
        StartCoroutine(WaitUntilFinishFading(LoadWaitingLevel));
    }

    private void FadeOut()
    {
        StartCoroutine(Fade(fadeOutLerpSpeed, false));
    }
    private IEnumerator WaitUntilFinishFading(Action action)
    {
        while(isFadingIn || isFadingOut)
        {
            yield return new WaitForEndOfFrame();
        }
        action.Invoke();
    }
    private IEnumerator Fade(float fadeLerpSpeed, bool state)
    {
        if ((!state && isFadingOut) || (state && isFadingIn))
        {
            yield break;
        }
        if (!state)
        {
            isFadingOut = true;
        }
        else
        {
            isFadingIn = true;
        }
        float startValue = 0;
        while (isFadingIn || isFadingOut)
        {
            startValue += fadeLerpSpeed;
            if (startValue > 1)
            {
                startValue = 1;
            }
            if (isFadingIn)
            {
                loadingScreenImage.color = new Color(loadingScreenImage.color.r, loadingScreenImage.color.g, loadingScreenImage.color.b, Mathf.InverseLerp(0, 1, startValue));
            }
            else if (!isFadingIn)
            {
                loadingScreenImage.color = new Color(loadingScreenImage.color.r, loadingScreenImage.color.g, loadingScreenImage.color.b, Mathf.InverseLerp(1, 0, startValue));
            }
            yield return new WaitForEndOfFrame();
            if (startValue == 1)
            {
                isFadingIn = false;
                isFadingOut = false;
            }
        }
    }
}
