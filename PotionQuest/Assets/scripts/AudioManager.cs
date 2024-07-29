using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource lightAmbient;
    public AudioSource shadowAmbient;
    private PlayerController playerController;

    [Range(0, 1)]
    public float Volume = 1.0f;

    [Range(0, 1)]
    public float lerpSpeed = 1.0f;


    private float audioLerp = 0.0f;
    

    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //start with light
        lightAmbient.volume = 0.5f;
        lightAmbient.Play();
        shadowAmbient.volume = 0;
        shadowAmbient.Play();
    }

    private void Update() {
        if (!playerController.inLight)
        {
            audioLerp += Time.deltaTime * lerpSpeed;
            if (audioLerp > 1) audioLerp = 1;
        }else{
            audioLerp -= Time.deltaTime * lerpSpeed;
            if (audioLerp < 0) audioLerp = 0;
        }
        lightAmbient.volume = Mathf.Lerp(Volume, 0, audioLerp);
        shadowAmbient.volume = Mathf.Lerp(0, Volume, audioLerp);
    }
    
    //switch the audio based on the player state

}
