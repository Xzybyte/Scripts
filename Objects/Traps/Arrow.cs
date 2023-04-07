using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private int targetsHit = 0;
    private int targetLimit = 1;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetsHit < targetLimit)
        {
            if (collision.tag == "Player")
            {
                targetsHit++;
                StartCoroutine(Destroy());
                collision.gameObject.GetComponent<Player>().TakeDamage(200);
            }
            else if (collision.tag == "Enemy")
            {
                targetsHit++;
                StartCoroutine(Destroy());
                collision.gameObject.GetComponent<Enemy>().TakeDamage(200, true);
            }
            else if (collision.tag == "Obstacle/Wall")
            {
                targetsHit++;
                StartCoroutine(Destroy());
            }
        }
    }

    private IEnumerator Destroy()
    {
        GetComponent<Animator>().SetBool("splash", true);
        this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.3f);
        Destroy(this.gameObject);
    }
}
