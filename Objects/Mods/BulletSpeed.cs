using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletSpeed : MonoBehaviour
{
    private GameObject player;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private PauseMenu pauseMenu;
    private int speedCost = 800;

    private void Start()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        player = GameObject.FindGameObjectWithTag("Player");
        pauseMenu = GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>();
    }

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
                if (player.GetComponent<Player>().GetCoins() < speedCost)
                {
                    player.GetComponent<Player>().StatusMessage("You do not have " + speedCost + " coins to upgrade your bullet speed.", 3);
                    return;
                }
                player.GetComponent<Player>().BulletSpeed(speedCost);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player.GetComponent<Player>().GetBulletSpeed() >= 30m)
            {
                player.GetComponent<Player>().DoorMessage("You cannot increase your bullet speed any further.");
            }
            else
            {
                player.GetComponent<Player>().DoorMessage("This bullet speed upgrader costs " + speedCost + " coins to use.\r\nPress the Interact button to use.");
            }
        }
        else if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }
}
