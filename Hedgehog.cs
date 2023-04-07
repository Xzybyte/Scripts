using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hedgehog : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health healthbar;
    private int maxHealth = 5000;
    private int currentHealth;

    private bool isDead;
    private bool isFacingLeft;
    private float accelerationTime = 2f;
    private float movementSpeed = 2f;
    private Vector2 movement;
    private float timeLeft;
    private GameObject player;

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        Physics2D.IgnoreCollision(player.GetComponent<CapsuleCollider2D>(), GetComponent<CircleCollider2D>());
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            animator.SetFloat("Speed", movement.sqrMagnitude);
            timeLeft += accelerationTime;
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }
        if (movement.x > 0 && isFacingLeft)
        {
            Flip();
        }
        else if (movement.x < 0 && !isFacingLeft)
        {
            Flip();
        }
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Interpolate the player's movement
        rb.MovePosition(rb.position + movement.normalized * movementSpeed * Time.fixedDeltaTime); // Move the player to position * speed
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        animator.Play("HedgeHog_Death_Animation");
    }

    public void DeathAnimation()
    {
        //Destroy(this.gameObject);
        player.GetComponent<Player>().Die();
    }

    public bool getFlip()
    {
        return isFacingLeft;
    }

    public void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        Vector3 hpScale = healthbar.transform.parent.localScale;
        hpScale.x *= -1;
        healthbar.transform.parent.localScale = hpScale;
    }
}
