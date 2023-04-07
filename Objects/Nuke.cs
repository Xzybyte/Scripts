using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuke : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ObjectFade());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Destroy(this.gameObject);
            player.GetComponent<Player>().StartNuke();
        }
    }

    private IEnumerator ObjectFade()
    {
        yield return new WaitForSeconds(20);
        GetComponent<Animator>().SetBool("fade", true);
    }
}
