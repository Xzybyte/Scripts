using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DmgMultiplier : MonoBehaviour
{
    private GameObject player;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private int multiplierCost = 450;
    private PauseMenu pauseMenu;

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
                if (player.GetComponent<Player>().GetCoins() < multiplierCost)
                {
                    player.GetComponent<Player>().StatusMessage("You do not have " + multiplierCost + " coins to upgrade your damage.", 3);
                    return;
                }
                player.GetComponent<Player>().DamageMultiplier(multiplierCost);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player.GetComponent<Player>().GetDamageMultiplier() >= 3m)
            {
                player.GetComponent<Player>().DoorMessage("You cannot increase your multiplier any further.");
            }
            else
            {
                player.GetComponent<Player>().DoorMessage("This damage multiplier upgrader costs " + multiplierCost + " coins to use.\r\nPress the Interact button to use.");
            }
        }
        else if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }
}