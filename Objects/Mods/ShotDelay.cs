using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShotDelay : MonoBehaviour
{
    private GameObject player;
    private int shotCost = 1500;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private PauseMenu pauseMenu;


    private void Start()
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
        if (Vector2.Distance(player.transform.position, this.transform.position) <= 1.5f)
        {
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame || gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
            {
                if (player.GetComponent<Player>().GetCoins() < shotCost)
                {
                    player.GetComponent<Player>().StatusMessage("You do not have " + shotCost + " coins to upgrade your shooting delay.", 3);
                    return;
                }
                player.GetComponent<Player>().ShotDelay(shotCost);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player.GetComponent<Player>().GetShootingDelay() >= 3m)
            {
                player.GetComponent<Player>().DoorMessage("You cannot increase your bullet speed any further.");
            }
            else
            {
                player.GetComponent<Player>().DoorMessage("This shooting delay upgrader costs " + shotCost + " coins to use.\r\nPress the Interact button to use.");
            }
        }
        else if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }
}
