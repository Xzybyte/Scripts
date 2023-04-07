using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{

    private EnemyRespawn enemyRespawn;
    private GameObject player;
    private int leverCost = 500;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private PauseMenu pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        enemyRespawn = GameObject.FindGameObjectWithTag("EnemyRespawner").GetComponent<EnemyRespawn>();
        player = GameObject.FindGameObjectWithTag("Player");
        pauseMenu = GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>();
    }

    private void Update()
    {
        if (pauseMenu.IsPaused())
        {
            return;
        }
        if (enemyRespawn.GetDarkness())
        {
            if (Vector2.Distance(player.transform.position, this.transform.position) <= 1f)
            {
                if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame || gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
                {
                    if (player.GetComponent<Player>().GetCoins() < leverCost)
                    {
                        player.GetComponent<Player>().StatusMessage("You do not have " + leverCost + " coins to use this lever.", 3);
                        return;
                    }
                    player.GetComponent<Player>().ClearMessage();
                    player.GetComponent<Player>().IncreaseCoins(-leverCost, false);
                    player.GetComponent<Player>().GetTaskSystem().CompleteTask(enemyRespawn.GetCurrentWave());
                    GetComponent<Animator>().SetBool("on", true);
                    enemyRespawn.Darkness(false);
                    StartCoroutine(RemoveLever());
                }
            }
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (enemyRespawn.GetDarkness())
            {
                player.GetComponent<Player>().DoorMessage("This lever costs " + leverCost + " coins to use.\r\nPress the Interact button to use.");
            }
        }
        else if (collision.collider.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator RemoveLever()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}
