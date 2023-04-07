using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallChestRandomizer : MonoBehaviour
{
    [SerializeField] private Sprite[] sprite;
    [SerializeField] private Animator animator;
    private int chosenBuff;

    public void TriggerAnimation()
    {
        animator.SetBool("randomizer", true);
    }

    public void RandomizeBuff()
    {
        animator.SetBool("randomizer", false);
        animator.enabled = false;
        int index = Random.Range(0, sprite.Length);
        Sprite chosenSprite = sprite[index];
        this.GetComponent<SpriteRenderer>().sprite = chosenSprite;
        chosenBuff = index;
    }

    public int getBuff()
    {
        return chosenBuff;
    }
}
