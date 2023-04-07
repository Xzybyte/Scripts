using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaleButton : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Image image;
    private Color white = new Color32(255, 255, 255, 255);
    private Color red = new Color32(255, 0, 0, 255);
    private bool chosen;
    private FemaleButton femaleButton;
    private GameObject playButton;

    void Start()
    {
        femaleButton = GameObject.FindGameObjectWithTag("FemaleButton").GetComponent<FemaleButton>();
        playButton = GameObject.FindGameObjectWithTag("PlayButton");
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Selected"))
        {
            chosen = true;
        } else
        {
            if (image.color != white)
            {
                image.color = white;
            }
        }
        if (chosen && image.color == white)
        {
            image.color = red;
            PlayMaleAnimation(true);
        } else if (!chosen && image.color == red)
        {
            image.color = white;
        }
    }

    public void PlayMaleAnimation(bool play)
    {
        if (play)
        {
            playButton.GetComponent<Button>().interactable = true;
            PlayerPrefs.SetString("Character", "male");
            femaleButton.PlayFemaleAnimation(false);
            animator.Play("Selected");
        } else
        {
            if (chosen)
            {
                chosen = false;
                animator.SetTrigger("Normal");
            }
        }
    }
}
