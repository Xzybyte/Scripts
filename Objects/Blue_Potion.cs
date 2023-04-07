using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue_Potion : MonoBehaviour
{
    private int potsGained = 1;

    private void Start()
    {
        StartCoroutine(ObjectFade());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            FindObjectOfType<AudioManager>().Play("pickup");
            Destroy(this.gameObject);
            int pot = potsGained;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().IncreaseBluePotions(pot);
        }
    }

    private IEnumerator ObjectFade()
    {
        yield return new WaitForSeconds(35);
        GetComponent<Animator>().SetBool("fade", true);
    }

    public void SetPotsGained(int pots)
    {
        potsGained = pots;
    }
}
