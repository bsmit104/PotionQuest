using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FastTravelInteractable : Interactable
{
    PlayerController player;
    private FastTravelManager manager;
    public GameObject FastTravelScreen;
    public TextMeshPro DestinationText;
    private bool open = false;

    public Interactable NextButton;
    public Interactable PreviousButton;
    public Interactable TeleportButton;


    private int SelectedIndex = 0;
    private List<FastTravelDestination> destinations;
    

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        manager = GameObject.FindGameObjectWithTag("FastTravelManager").GetComponent<FastTravelManager>();
        OnInteract += OpenOrCloseTravelScreen;
        NextButton.OnInteract += Next;
        PreviousButton.OnInteract += Previous;
        TeleportButton.OnInteract += Teleport;

    }

    public void Next()
    {
        SelectedIndex++;
        if (SelectedIndex >= destinations.Count)
        {
            SelectedIndex = 0;
        }
        if (destinations.Count > 0)
            DestinationText.text = destinations[SelectedIndex].DestinationName;
    }

    public void Previous()
    {
        SelectedIndex--;
        if (SelectedIndex < 0)
        {
            SelectedIndex = destinations.Count - 1;
        }
        if (destinations.Count > 0)
            DestinationText.text = destinations[SelectedIndex].DestinationName;
    }

    public void Teleport()
    {
        if (destinations.Count > 0)
        {
            player.Teleport(destinations[SelectedIndex].TeleportToPosition);
        }
    }

    public void OpenOrCloseTravelScreen()
    {
        if (!open)
        {
            //open screen
            open = true;
            destinations = manager.GetDestinations();
            if (destinations.Count > 0)
                DestinationText.text = destinations[SelectedIndex].DestinationName;
            FastTravelScreen.SetActive(true);
        }else
        {
            //need to close
            open = false;
            FastTravelScreen.SetActive(false);
        }
    }
    
    public override string GetHoveredText()
    {   
        if (!open)
        {
            return "Open Fast Travel Menu";
        }else
        {
            return "Close Fast Travel Menu";
        }
    }

    private void Update() {
        //FastTravelScreen.transform.LookAt(player.transform, Vector3.up);
    }
}
