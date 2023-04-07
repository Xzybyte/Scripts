using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private int coinsMin = 0;
    private int coinsMax = 0;

    private void Start()
    {
        StartCoroutine(ObjectFade());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            Destroy(this.gameObject);
            FindObjectOfType<AudioManager>().Play("coin");
            int coins = Random.Range(coinsMin, coinsMax);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().IncreaseCoins(coins, true);
        }
    }

    private IEnumerator ObjectFade()
    {
        yield return new WaitForSeconds(35);
        GetComponent<Animator>().SetBool("fade", true);
    }

    public void SetMinCoin(int c)
    {
        this.coinsMin = c;
    }

    public void SetMaxCoin(int c)
    {
        this.coinsMax = c;
    }
}
