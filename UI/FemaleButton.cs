using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FemaleButton : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image image;
    private Color white = new Color32(255, 255, 255, 255);
    private Color red = new Color32(255, 0, 0, 255);
    private bool chosen;
    private MaleButton maleButton;
    private GameObject playButton;

    void Start()
    {
        maleButton = GameObject.FindGameObjectWithTag("MaleButton").GetComponent<MaleButton>();
        playButton = GameObject.FindGameObjectWithTag("PlayButton");
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Selected"))
        {
            chosen = true;
        }
        else
        {
            if (image.color != white)
            {
                image.color = white;
            }
        }
        if (chosen && image.color == white)
        {
            image.color = red;
            PlayFemaleAnimation(true);
        }
        else if (!chosen && image.color == red)
        {
            image.color = white;
        }
    }

    public void PlayFemaleAnimation(bool play)
    {
        if (play)
        {
            playButton.GetComponent<Button>().interactable = true;
            PlayerPrefs.SetString("Character", "female");
            maleButton.PlayMaleAnimation(false);
            animator.Play("Selected");
        }
        else
        {
            if (chosen)
            {
                chosen = false;
                animator.SetTrigger("Normal");
            }
        }
    }
}
