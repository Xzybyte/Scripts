using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FireTrap : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject fire;
    [SerializeField] private Text fireText;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private bool charged = false;
    private int fireTrapCost = 700;
    private int shotsTaken = 0;
    private PauseMenu pauseMenu;

    void Start()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        player = GameObject.FindGameObjectWithTag("Player");
        pauseMenu = GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu.IsPaused())
        {
            return;
        }
        if (!charged)
        {
            if (Vector2.Distance(player.transform.position, this.transform.position) <= 1.5f)
            {
                if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame || gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
                {
                    if (player.GetComponent<Player>().GetCoins() < fireTrapCost)
                    {
                        player.GetComponent<Player>().StatusMessage("You do not have " + fireTrapCost + " coins to turn on this fire trap.", 3);
                        return;
                    }
                    player.GetComponent<Player>().DoorMessage("This fire trap will start firing in 3 seconds.");
                    player.GetComponent<Player>().IncreaseCoins(-fireTrapCost, false);
                    charged = true;
                    StartCoroutine(Fire());
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            player.GetComponent<Player>().DoorMessage("This fire trap costs " + fireTrapCost + " coins to use. (40 charges)\r\nPress the Interact button to use.");
        }
        else if (collision.collider.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            if (shotsTaken < 40)
            {
                yield return new WaitForSeconds(2f);
                Instantiate(fire, transform.position, Quaternion.identity, transform);
                shotsTaken++;
                fireText.text = (40 - shotsTaken) + " charges left.";
            } else
            {
                shotsTaken = 0;
                fireText.text = "Fire Trap";
                charged = false;
                break;
            }
        }
    }
}
