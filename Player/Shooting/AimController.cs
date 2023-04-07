using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bullet;
    private GameObject customGamepadCursor;
    private Mouse mouse;
    private Gamepad gamepad;
    private float distanceFromTarget = 1f;

    private Vector2 velocity = Vector2.zero;
    private bool mouseMoved;
    private bool gamepadMoved;
    private CanvasScaler scaler;
    private bool locked;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Debug.Log(string.Join("\n", Gamepad.all));
        mouse = InputSystem.GetDevice<Mouse>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        player = GameObject.FindGameObjectWithTag("Player");
        customGamepadCursor = GameObject.FindGameObjectWithTag("CustomCursor");
        scaler = customGamepadCursor.GetComponentInParent<CanvasScaler>();
    }

    void FixedUpdate()
    {
        Vector2 scalerRef = new Vector2(scaler.referenceResolution.x / Screen.width, scaler.referenceResolution.y / Screen.height);
        if (mouse != null)
        {
            float x = mouse.delta.x.ReadValue();
            float y = mouse.delta.y.ReadValue();
            if (x != 0|| y != 0)
            {
                mouseMoved = true;
            } else
            {
                mouseMoved = false;
            }
            if (mouseMoved)
            {
                x *= player.GetComponent<Player>().GetMouseSensitivity().x;
                y *= player.GetComponent<Player>().GetMouseSensitivity().y;
                Vector2 customGamepadPosition = customGamepadCursor.GetComponent<RectTransform>().anchoredPosition;
                Vector2 cursorPos = new Vector2(customGamepadPosition.x + x * scalerRef.x, customGamepadPosition.y + y * scalerRef.y);
                if (InsideBounds(cursorPos, customGamepadPosition))
                {
                    return;
                }
                float smoothTime = player.GetComponent<Player>().GetMouseSensitivity().x / 100;
                customGamepadCursor.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(customGamepadPosition, cursorPos, ref velocity, smoothTime);

            }
        }
        if (gamepad != null && !mouseMoved)
        {
            float x = gamepad.rightStick.x.ReadValue();
            float y = gamepad.rightStick.y.ReadValue();
            if (x != 0 || y != 0)
            {
                gamepadMoved = true;
            } else
            {
                gamepadMoved = false;
            }
            if (gamepadMoved)
            {
                x *= player.GetComponent<Player>().GetGamepadSensitivity().x;
                y *= player.GetComponent<Player>().GetGamepadSensitivity().y;
                Vector2 customGamepadPosition = customGamepadCursor.GetComponent<RectTransform>().anchoredPosition;
                Vector2 cursorPos = new Vector2(customGamepadPosition.x + x * scalerRef.x, customGamepadPosition.y + y * scalerRef.y);
                if (InsideBounds(cursorPos, customGamepadPosition))
                {
                    return;
                }
                customGamepadCursor.GetComponent<RectTransform>().anchoredPosition = cursorPos;
            }
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        Vector3 mousePos = new Vector3(customGamepadCursor.transform.position.x, customGamepadCursor.transform.position.y, 0);
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(player.transform.position);
        Vector3 targetToMouseDirection = mousePos - objectPos;
        mousePos.x -= objectPos.x;
        mousePos.y -= objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        float gunAngle = angle;
        if (playerMovement.getFlip())
        {
            gunAngle = (180 + angle) % 360;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, gunAngle));

        Vector3 targetToMe = transform.position - player.transform.position;
        Vector3 newTargetToMe = Vector3.RotateTowards(targetToMe, targetToMouseDirection, 10f, 0f);
        transform.position = player.transform.position + distanceFromTarget * newTargetToMe.normalized;
        if (angle >= -90f && angle <= 90f)
        {
            if (playerMovement.getFlip())
            {
                player.GetComponent<PlayerMovement>().Flip();
            }
        } else
        {
            if (!playerMovement.getFlip())
            {
                player.GetComponent<PlayerMovement>().Flip();
            }
        }
    }

    private bool InsideBounds(Vector2 cursorPos, Vector2 customGamepadPosition)
    {
        if (customGamepadPosition.x >= 1920 || customGamepadPosition.x <= 0 || customGamepadPosition.y >= 1080 || customGamepadPosition.y <= 0)
        {
            if (cursorPos.x <= 0)
            {
                customGamepadCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(1, customGamepadPosition.y);
                return true;
            }
            else if (cursorPos.x >= 1920)
            {
                customGamepadCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(1919, customGamepadPosition.y);
                return true;
            }

            if (cursorPos.y <= 0)
            {
                customGamepadCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(customGamepadPosition.x, 1);
                return true;
            }
            else if (cursorPos.y >= 1080)
            {
                customGamepadCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(customGamepadPosition.x, 1079);
                return true;
            }
        }
        return false;
    }

    public GameObject getCurrentBullet()
    {
        return bullet;
    }
}
