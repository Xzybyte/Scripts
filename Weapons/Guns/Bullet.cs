using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private int bulletDamage = 0;
    private int targetsHit = 0;
    private int targetLimit = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Destroy(this.gameObject);
            if (targetsHit < targetLimit)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int magnitude = enemy.getKnockback();
                    //Debug.Log(magnitude);
                    if (magnitude > 0)
                    {
                        Vector3 force = transform.position - enemy.transform.position;
                        force.Normalize();
                        enemy.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
                    }
                    if (enemy != null)
                    {
                        targetsHit++;
                        enemy.TakeDamage(bulletDamage, false);
                    }
                }
            }
        } else if (collision.collider.tag == "Obstacle/Wall" || collision.collider.tag == "Bullet" || collision.collider.tag == "Chest")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }

    public void setBulletDamage(int damage)
    {
        bulletDamage = damage;
    }
}
