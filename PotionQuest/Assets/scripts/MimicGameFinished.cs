using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class MimicGameFinished : MonoBehaviour
{
    public float animationSpeed = 0.75f;
    private PlayerController PlayerController;
    public PauseMenu menu;

    //lid
    public GameObject chestTop;
    public Vector3 ChestClosedRotation;
    public Vector3 ChestOpenedRotation;

    //tongue
    public GameObject Tongue;
    public Vector3 tongueStartScale;
    public Vector3 tongueEndScale;


    //pull
    private Vector3 playerStartPosition;

    //game finished

    private bool chestIsOpen = false;
    private float lerp = 0;

    private void OnEnable() {
        chestIsOpen = true;
        PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerStartPosition = PlayerController.transform.position;

        //stop player from moving
        PlayerController.canMove = false;
        PlayerController.GetComponent<CharacterController>().enabled = false;
    }
    private void Update() {
        if (chestIsOpen)
        {
            //increase animation time
            lerp += Time.deltaTime * animationSpeed;

            //stage 0: open lid
            chestTop.transform.rotation = Quaternion.Euler(Vector3.Lerp(ChestClosedRotation, ChestOpenedRotation, GetLerpStage(0)));

            //stage 1: scale tongue
            //point tongue at player
            if (lerp >= 1)
                Tongue.transform.LookAt(PlayerController.transform);
            Tongue.transform.localScale = Vector3.Lerp(tongueStartScale, tongueEndScale, GetLerpStage(1));

            //stage 2: pull player
            if (lerp >= 2)
            {
                //pull player
                PlayerController.transform.position = Vector3.Lerp(playerStartPosition, transform.position, GetLerpStage(2));
                //shrink tongue while pulling
                Tongue.transform.localScale = transform.localScale + Vector3.Lerp(tongueEndScale, tongueStartScale, GetLerpStage(2));
            }

            //stage 3: game finished
            if (lerp >= 3)
            {
                menu.GameFinished();
            }

        }
    }

    private float GetLerpStage(float stage)
    {
        
        return Mathf.Clamp(lerp - stage, 0, 1);
    }
}
