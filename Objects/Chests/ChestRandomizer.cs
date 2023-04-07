using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRandomizer : MonoBehaviour
{

    [SerializeField] private Sprite[] sprite;
    [SerializeField] private Animator animator;
    private int chosenWeapon;
    
    public void TriggerAnimation()
    {
        animator.SetBool("randomizer", true);
    }

    public void RandomizeWeapon(int doorSize)
    {
        animator.SetBool("randomizer", false);
        animator.enabled = false;
        int index = Random.Range(1, doorSize);
        Sprite chosenSprite = sprite[index];
        this.GetComponent<SpriteRenderer>().sprite = chosenSprite;
        chosenWeapon = index;
    }

    public int getWeapon()
    {
        return chosenWeapon;
    }
}
