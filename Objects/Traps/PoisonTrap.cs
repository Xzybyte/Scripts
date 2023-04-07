using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PoisonTrap : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject poison;
    [SerializeField] private Text poisonText;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private bool charged = false;
    private int poisonTrapCost = 900;
    private int shotsTaken = 0;
    private PauseMenu pauseMenu;

    // Start is called before the first frame update
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
                    if (player.GetComponent<Player>().GetCoins() < poisonTrapCost)
                    {
                        player.GetComponent<Player>().StatusMessage("You do not have " + poisonTrapCost + " coins to turn on this poison trap.", 3);
                        return;
                    }
                    player.GetComponent<Player>().DoorMessage("This poison trap will start firing in 3 seconds.");
                    player.GetComponent<Player>().IncreaseCoins(-poisonTrapCost, false);
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
            player.GetComponent<Player>().DoorMessage("This poison trap costs " + poisonTrapCost + " coins to use. (50 charges)\r\nPress the Interact button to use.");
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
            if (shotsTaken < 50)
            {
                yield return new WaitForSeconds(3f);
                Instantiate(poison, transform.position, Quaternion.identity, transform);
                shotsTaken++;
                poisonText.text = (50 - shotsTaken) + " charges left.";
            }
            else
            {
                shotsTaken = 0;
                charged = false;
                poisonText.text = "Poison Trap";
                break;
            }
        }
    }
}
