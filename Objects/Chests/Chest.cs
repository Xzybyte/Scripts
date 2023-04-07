using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{

    private bool stopDestroy = false;
    private bool canLoot = false;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject indicator;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private ChestRandomizer chestRandomizer;
    private GameObject player;
    private int coinCost = 100;
    private PauseMenu pauseMenu;

    private void Start()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        player = GameObject.FindGameObjectWithTag("Player");
        chestRandomizer = transform.GetChild(0).GetComponent<ChestRandomizer>();
        pauseMenu = GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>();
    }

    private void Update()
    {
        if (pauseMenu.IsPaused())
        {
            return;
        }
        if (!animator.GetBool("opened"))
        {
            if (Vector2.Distance(player.transform.position, this.transform.position) <= 1f)
            {
                if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame || gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
                {
                    indicator.GetComponent<Text>().text = "";
                    if (player.GetComponent<Player>().GetCoins() < coinCost)
                    {
                        player.GetComponent<Player>().StatusMessage("You do not have " + coinCost + " coins to open this chest.", 3);
                        return;
                    }
                    player.GetComponent<Player>().IncreaseCoins(-coinCost, false);
                    stopDestroy = true;
                    animator.SetBool("opened", true);
                    StartCoroutine(ChooseWeapon(1.2f));
                }
            }
        }
        if (Vector2.Distance(player.transform.position, this.transform.position) <= 1f && canLoot)
        {
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame || gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
            {
                player.GetComponent<Player>().UpdateGun(chestRandomizer.getWeapon());
                GameObject.FindGameObjectWithTag("ChestRespawner").GetComponent<ChestRespawner>().ResetTimer();
                Destroy(this.gameObject);
                Destroy(chestRandomizer.gameObject);
            }
        }
    }

    IEnumerator ChooseWeapon(float quickWait)
    {
        yield return new WaitForSeconds(quickWait);
        chestRandomizer.TriggerAnimation();
        yield return new WaitForSeconds(3);
        chestRandomizer.RandomizeWeapon(GameObject.FindGameObjectWithTag("EnemyRespawner").GetComponent<EnemyRespawn>().DoorSize());
        canLoot = true;
        indicator.GetComponent<Text>().text = "Press the Interact button to pick up '" + player.GetComponent<Player>().getNameByGun(chestRandomizer.getWeapon()) + "'.";
    }

    public bool StopDestroy()
    {
        return stopDestroy;
    }
}
