using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectralSword : MonoBehaviour
{

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Vector2 movement;
    private float accelerationTime = 2f;
    private float movementSpeed = 6f;
    private float timeLeft;
    private bool reachedPosition;
    private bool charged = true;

    // Start is called before the first frame update
    void Start()
    {
        if (charged)
        {
            movementSpeed = 9f;
        }
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        targetPosition = new Vector3(transform.localScale.x < 0 ? startPosition.x - 1.5f : startPosition.x + 1.5f, startPosition.y - 1.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            movement = new Vector3(transform.localScale.x < 0 ? -.5f : +.5f, -.5f, 0);
            //animator.SetFloat("Speed", movement.sqrMagnitude);
            timeLeft += accelerationTime;
        }
    }

    private void FixedUpdate()
    {
        if (reachedPosition)
        {
            return;
        }
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Interpolate the player's movement
        rb.MovePosition(rb.position + movement.normalized * movementSpeed * Time.fixedDeltaTime); // Move the player to position * speed
        if (transform.localScale.x < 0 ? targetPosition.x >= transform.position.x && targetPosition.y >= transform.position.y : targetPosition.x <= transform.position.x && targetPosition.y >= transform.position.y)
        {
            reachedPosition = true;
        }
        if (reachedPosition) { 
            GetComponent<PolygonCollider2D>().enabled = true;
            StartCoroutine(RemoveSword());
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Player>().TakeDamage(100);
            GetComponent<Animator>().SetBool("hitPlayer", true);
            Destroy(this.gameObject, 0.65f);
        }
    }

    private IEnumerator RemoveSword()
    {
        yield return new WaitForSeconds(0.3f);
        GetComponent<Animator>().SetBool("hitPlayer", true);
        yield return new WaitForSeconds(0.8f);
        Destroy(this.gameObject);
    }

    public void Charged(bool charged)
    {
        this.charged = charged;
    }

}
