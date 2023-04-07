using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SmallChest : MonoBehaviour
{
    private bool stopDestroy = false;
    private bool canLoot = false;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject indicator;
    private SmallChestRandomizer smallChestRandomizer;
    private GameObject player;
    private int coinCost = 400;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private PauseMenu pauseMenu;

    private void Start()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        player = GameObject.FindGameObjectWithTag("Player");
        smallChestRandomizer = transform.GetChild(0).GetComponent<SmallChestRandomizer>();
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
                player.GetComponent<Player>().GiveBuff(smallChestRandomizer.getBuff());
                GameObject.FindGameObjectWithTag("ChestRespawner").GetComponent<ChestRespawner>().ResetTimer();
                Destroy(this.gameObject);
                Destroy(smallChestRandomizer.gameObject);
            }
        }
    }

    IEnumerator ChooseWeapon(float quickWait)
    {
        yield return new WaitForSeconds(quickWait);
        smallChestRandomizer.TriggerAnimation();
        yield return new WaitForSeconds(3);
        smallChestRandomizer.RandomizeBuff();
        canLoot = true;
        indicator.GetComponent<Text>().text = "Press the Interact button to pick up '" + player.GetComponent<Player>().getNameByBuff(smallChestRandomizer.getBuff()) + "'.";
    }

    public bool StopDestroy()
    {
        return stopDestroy;
    }
}
