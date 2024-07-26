using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookMenuController : MonoBehaviour
{
    public Image[] pages; // Array to hold the pages
    public Button iconButton; // Icon button to open/close the menu

    private int currentPage = 0;
    private bool menuOpen = false;

    void Start()
    {
        iconButton.onClick.AddListener(ToggleMenu);
        UpdatePages();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }

        if (menuOpen)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextPage();
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousPage();
            }
        }
    }

    void ToggleMenu()
    {
        if (menuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    void OpenMenu()
    {
        menuOpen = true;
        iconButton.gameObject.SetActive(true); // Keep the icon button active for toggling
        UpdatePages();
    }

    void CloseMenu()
    {
        menuOpen = false;
        iconButton.gameObject.SetActive(true); // Ensure the icon button is visible when menu is closed
        UpdatePages();
    }

    void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            UpdatePages();
        }
    }

    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePages();
        }
    }

    void UpdatePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].gameObject.SetActive(i == currentPage && menuOpen);
        }
    }
}
