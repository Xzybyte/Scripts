using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{

    private float movementSpeed = 4.5f;
    //public AudioSource audioSource;
    private bool isMoving;

    [SerializeField] private GameObject overview;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Text leverText;

    private EnemyRespawn enemyRespawn;
    private PauseMenu pauseMenu;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private Vector2 movement;
    private bool isFacingLeft = false;

    //[SerializeField] private float stepRate = 0.5f;
    [SerializeField] private float stepCoolDown;
    private float hintTimer;

    private void Start()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        enemyRespawn = GameObject.FindGameObjectWithTag("EnemyRespawner").GetComponent<EnemyRespawn>();
        pauseMenu = GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>();
        overview.GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu.IsPaused())
        {
            return;
        }
        Cursor.visible = false;
        if (keyboard != null && keyboard.tabKey.wasPressedThisFrame || gamepad != null && gamepad.leftShoulder.wasPressedThisFrame)
        {
            ShowMap(true);
        } else if (keyboard != null && keyboard.tabKey.wasReleasedThisFrame || gamepad != null && gamepad.leftShoulder.wasReleasedThisFrame)
        {
            ShowMap(false);
        }

        bool keyPressed = false;
        if (keyboard != null)
        {
            bool movePlayer = MovePlayer();
            keyPressed = movePlayer;
            if (!movePlayer)
            {
                movement.x = 0;
                movement.y = 0;
            }
        }
        if (gamepad != null)
        {
            if (!keyPressed)
            {
                movement.x = gamepad.leftStick.x.ReadValue();
                movement.y = gamepad.leftStick.y.ReadValue();
            }
        }
        animator.SetFloat("Speed", movement.sqrMagnitude);
        if (enemyRespawn.GetDarkness())
        {
            if (enemyRespawn.GetLeverInstance() != null)
            {
                hintTimer += Time.deltaTime;
                if (hintTimer > 3)
                {
                    int distance = (int) Vector2.Distance(rb.position, enemyRespawn.GetLeverInstance().GetComponent<Rigidbody2D>().position);
                    leverText.text = "Lever distance: " + distance;
                    hintTimer = 0;
                }                
            }
        }
    }

    private bool MovePlayer()
    {
        bool movePlayer = false;
        KeyControl[] upKeys = { keyboard.upArrowKey, keyboard.wKey };
        KeyControl[] downKeys = { keyboard.downArrowKey, keyboard.sKey };
        KeyControl[] leftKeys = { keyboard.leftArrowKey, keyboard.aKey };
        KeyControl[] rightKeys = { keyboard.rightArrowKey, keyboard.dKey };

        for (int i = 0; i < upKeys.Length; i++)
        {
            if (upKeys[i].isPressed)
            {
                movement.y = 1;
                movePlayer = true;
                break;
            } else if (upKeys[i].wasReleasedThisFrame)
            {
                movement.y = 0;
                movePlayer = true;
                break;
            }
        }

        for (int i = 0; i < downKeys.Length; i++)
        {
            if (downKeys[i].isPressed)
            {
                movement.y = -1;
                movePlayer = true;
                break;
            }
            else if (downKeys[i].wasReleasedThisFrame)
            {
                movement.y = 0;
                movePlayer = true;
                break;
            }
        }

        for (int i = 0; i < leftKeys.Length; i++)
        {
            if (leftKeys[i].isPressed)
            {
                movement.x = -1;
                movePlayer = true;
                break;
            }
            else if (leftKeys[i].wasReleasedThisFrame)
            {
                movement.x = 0;
                movePlayer = true;
                break;
            }
        }

        for (int i = 0; i < rightKeys.Length; i++)
        {
            if (rightKeys[i].isPressed)
            {
                movement.x = 1;
                movePlayer = true;
                break;
            }
            else if (rightKeys[i].wasReleasedThisFrame)
            {
                movement.x = 0;
                movePlayer = true;
                break;
            }
        }

        return movePlayer;
    }

    private void FixedUpdate()
    {
        if (GetComponent<Player>().GetMovementSpeed())
        {
            movementSpeed = 7.5f;
        } else
        {
            movementSpeed = 4.5f;
        }
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Interpolate the player's movement
        rb.MovePosition(rb.position + movement.normalized * movementSpeed * Time.fixedDeltaTime); // Move the player to position * speed
    }

    public void ShowMap(bool show)
    {
        overview.GetComponent<Canvas>().enabled = show;
    }

    public bool getFlip()
    {
        return isFacingLeft;
    }

    public void Flip()
    {
        if (!GetComponent<Player>().getDead())
        {
            isFacingLeft = !isFacingLeft;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
