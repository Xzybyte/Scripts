using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.InputSystem;

public class Doors : MonoBehaviour
{

    private GameObject door;
    private GameObject gate1;
    private GameObject gate2;
    private GameObject player;
    private int doorCost = 150;
    private bool unlocked = false;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private PauseMenu pauseMenu;


    private void Start()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        door = this.gameObject;
        gate1 = transform.GetChild(0).gameObject;
        gate2 = transform.GetChild(1).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        pauseMenu = GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>();
    }

    private void Update()
    {
        if (pauseMenu.IsPaused())
        {
            return;
        }
        if (this.tag.Contains("Door") && !unlocked)
        {
            if (Vector2.Distance(player.transform.position, this.transform.position) <= 1.5f)
            {
                if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame || gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
                {
                    if (player.GetComponent<Player>().GetCoins() < doorCost)
                    {
                        player.GetComponent<Player>().StatusMessage("You do not have " + doorCost + " coins to open this door.", 3);
                        return;
                    }
                    player.GetComponent<Player>().ClearMessage();
                    player.GetComponent<Player>().IncreaseCoins(-doorCost, false);
                    StartCoroutine(UnlockDoor());
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (this.tag.Contains("Door"))
            {
                player.GetComponent<Player>().DoorMessage("Doors in this room costs " + doorCost + " coins to open.\r\nPress the Interact button to open.");
            }
        } else if (collision.collider.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator UnlockDoor()
    {
        FindObjectOfType<AudioManager>().Play("door");
        unlocked = true;
        door.GetComponent<Animator>().SetBool("opened", true);
        gate1.GetComponent<Animator>().SetBool("opened", true);
        gate2.GetComponent<Animator>().SetBool("opened", true);
        yield return new WaitForSeconds(0.8f);
        door.GetComponent<BoxCollider2D>().enabled = false;
        gate1.GetComponent<BoxCollider2D>().enabled = false;
        gate2.GetComponent<BoxCollider2D>().enabled = false;
        int defaultLayer = LayerMask.NameToLayer("Default");
        door.layer = defaultLayer;
        gate1.layer = defaultLayer;
        gate2.layer = defaultLayer;
        GameObject.FindGameObjectWithTag("EnemyRespawner").GetComponent<EnemyRespawn>().AddToDoors(System.Int32.Parse(this.tag.Substring(4)));
        UnlockAdjacentDoors();
        AstarPath.active.Scan();
    }

    private void UnlockAdjacentDoors()
    {
        GameObject[] gObjects = GameObject.FindGameObjectsWithTag(this.tag);

        for (int i = 0; i < gObjects.Length; i++)
        {
            GameObject doorObj = gObjects[i];
            GameObject gateObj1 = gObjects[i].transform.GetChild(0).gameObject;
            GameObject gateObj2 = gObjects[i].transform.GetChild(1).gameObject;
            doorObj.GetComponent<Animator>().SetBool("opened", true);
            gateObj1.GetComponent<Animator>().SetBool("opened", true);
            gateObj2.GetComponent<Animator>().SetBool("opened", true);
            doorObj.GetComponent<BoxCollider2D>().enabled = false;
            gateObj1.GetComponent<BoxCollider2D>().enabled = false;
            gateObj2.GetComponent<BoxCollider2D>().enabled = false;
            int defaultLayer = LayerMask.NameToLayer("Default");
            doorObj.layer = defaultLayer;
            gateObj1.layer = defaultLayer;
            gateObj2.layer = defaultLayer;
        }
    }

    /*private int GetCostByDoor(string doorTag)
    {
        string[] doors = { "Door2", "Door3", "Door4", "Door5", "Door6", "Door7", "Door8", "Door9", "Door10", "Door11"};
        int[] costs = { 200, 350, 500, 600, 750, 900, 1050, 1200, 1500, 1700};

        int returnCost = 0;

        for (int i = 0; i < doors.Length; i++)
        {
            if (doorTag == doors[i])
            {
                returnCost = costs[i];
                break;
            }
        }
        return returnCost;
    }*/
}
