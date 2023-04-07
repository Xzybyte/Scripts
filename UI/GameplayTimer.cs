using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayTimer : MonoBehaviour
{
    [SerializeField] private Text gameTimerText;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer % 60F);
        gameTimerText.text = "Total Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
