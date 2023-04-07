using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ArrowTrap : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Text arrowText;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private bool charged = false;
    private int arrowTrapCost = 500;
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
    private void Update()
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
                    if (player.GetComponent<Player>().GetCoins() < arrowTrapCost)
                    {
                        player.GetComponent<Player>().StatusMessage("You do not have " + arrowTrapCost + " coins to turn on this arrow trap.", 3);
                        return;
                    }
                    player.GetComponent<Player>().DoorMessage("This arrow trap will start firing in 3 seconds.");
                    player.GetComponent<Player>().IncreaseCoins(-arrowTrapCost, false);
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
            player.GetComponent<Player>().DoorMessage("This arrow trap costs " + arrowTrapCost + " coins to use. (30 charges)\r\nPress the Interact button to use.");
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
            if (shotsTaken < 30)
            {
                yield return new WaitForSeconds(1f);
                Instantiate(arrow, transform.position, Quaternion.identity, transform);
                shotsTaken++;
                arrowText.text = (30 - shotsTaken) + " charges left.";
            }
            else
            {
                shotsTaken = 0;
                arrowText.text = "Arrow Trap";
                charged = false;
                break;
            }
        }
    }
}
