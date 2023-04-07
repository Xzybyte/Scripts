using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float smoothTime = 1f;
    private bool male;

    private void Awake()
    {
        male = PlayerPrefs.GetString("Character").Equals("male");
    }

    // Start is called before the first frame update
    private void LateUpdate()
    {
        Vector3 cameraNewPosition = Vector3.zero;
        if (male)
        {
            cameraNewPosition = new Vector3(player1.position.x, player1.position.y, transform.position.z); // Create a new vector from the target position and camera z
        }
        else
        {
            cameraNewPosition = new Vector3(player2.position.x, player2.position.y, transform.position.z); // Create a new vector from the target position and camera z
        }
        transform.position = Vector3.SmoothDamp(transform.position, cameraNewPosition, ref velocity, smoothTime); // Set the camera's position
    }
}
