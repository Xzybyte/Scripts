using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CancelOptions : MonoBehaviour
{

    [SerializeField] private PauseMenu pauseMenu;
    private Gamepad gamepad;

    public void Awake()
    {
        gamepad = InputSystem.GetDevice<Gamepad>();
    }

    public void Update()
    {
        if (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        transform.parent.gameObject.SetActive(false);
        pauseMenu.PauseGame();
    }
}
