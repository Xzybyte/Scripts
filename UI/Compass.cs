using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    private Transform playerPosition;
    private EnemyRespawn enemyRespawn;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        enemyRespawn = GameObject.FindGameObjectWithTag("EnemyRespawner").GetComponent<EnemyRespawn>();
        playerPosition = player.transform;
    }

    void Update()
    {
        if (!enemyRespawn.GetDarkness())
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
            Transform target = null;
            Vector3 newPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, playerPosition.position.z);
            Vector3 direction = Vector3.zero;
            float distance = Mathf.Infinity;

            //Debug.Log(targets.Length);
            for (int i = 0; i < targets.Length; i++)
            {
                Vector2 thisDirection = targets[i].transform.position - newPosition;
                float thisDistance = thisDirection.sqrMagnitude;
                if (thisDistance < distance)
                {
                    direction = thisDirection;
                    distance = thisDistance;
                    target = targets[i].transform;
                }
            }
            if (direction != Vector3.zero)
            {
                transform.up = target.position - playerPosition.position;
            }
        }
    }
}