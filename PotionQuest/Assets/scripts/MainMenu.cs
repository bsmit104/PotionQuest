using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This function will be called when the Play button is clicked
    public void PlayGame()
    {
        // Load the scene named "game"
        SceneManager.LoadScene("CastleScene");
    }
}