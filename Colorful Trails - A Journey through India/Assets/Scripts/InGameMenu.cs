using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject settingsButton;
    public GameObject restartButton;
    public GameObject soundButton;
    public GameObject QuitButton;

    public Sprite soundONSprite;
    public Sprite soundOFFSprite;

    private bool isAnimationPlaying;
    private Animator animationHandler;

    void Start()
    {
        restartButton.SetActive(false);
        soundButton.SetActive(false);
        QuitButton.SetActive(false);

        animationHandler = GetComponent<Animator>();
        isAnimationPlaying = false;
    }

    public void SettingsButtonClicked()
    {
        restartButton.SetActive(true);
        soundButton.SetActive(true);
        QuitButton.SetActive(true);

        //Play or Stop animation
        if (!isAnimationPlaying)
        {
            animationHandler.SetTrigger("PlayAnimation");
        }
        else
        {
            animationHandler.SetTrigger("StopAnimation");
        }

        isAnimationPlaying = !isAnimationPlaying;
    }
    
    public void ToggleSound()
    {
        // Code for toggling the sound

        // Get the Image component of the soundButton
        Image soundButtonImage = soundButton.GetComponent<Image>();

        //changing the sprite from soundONButton to soundOFFButton
        if(soundButtonImage.sprite == soundONSprite)
        {
            soundButtonImage.sprite = soundOFFSprite;
        }
        else
        {
            soundButtonImage.sprite = soundONSprite;
        }
    }
}
