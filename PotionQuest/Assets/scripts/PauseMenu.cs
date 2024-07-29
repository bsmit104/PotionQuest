using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject GameFinishedUI;
    public PlayerController playerController;
    public GameObject Book;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        playerController.LockCursor();
        isPaused = false;
    }

    public void GameFinished()
    {
        GameFinishedUI.SetActive(true);
        Book.SetActive(false);
        Time.timeScale = 0f;
        playerController.UnlockCursor();
        isPaused = true;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        playerController.UnlockCursor();
        isPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}