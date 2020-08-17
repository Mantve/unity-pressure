using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject leaderboard;
    [SerializeField]
    private string newGameLevelName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!mainMenu.activeInHierarchy)
            {
                mainMenu.SetActive(true);
                leaderboard.SetActive(false);
            }
        }
    }

    public void OnNewGameButtonClick()
    {
        LevelManager.Instance.LoadOnFullyFadded(newGameLevelName);
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
