using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Startup : MonoBehaviour
{

    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private Text startMessage;
    private Resolution[] resolutions;

    // Start is called before the first frame update
    void Awake()
    {
        resolutions = Screen.resolutions;
        bool male = PlayerPrefs.GetString("Character").Equals("male");
        if (!male)
        {
            player1.gameObject.SetActive(false);
            player1.gameObject.tag = "Untagged";
            player2.gameObject.SetActive(true);
        }
        StartCoroutine(StartMessage());
    }
    public void MakeWindowed()
    {
        Screen.SetResolution(1280, 720, false);
    }

    public void MakeFullScreen()
    {
        int maxWidth = resolutions[resolutions.Length - 1].width;
        int maxHeight = resolutions[resolutions.Length - 1].height;
        if (maxWidth > 1920 || maxHeight > 1080)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Screen.SetResolution(maxWidth, maxHeight, true);
        }
    }

    private IEnumerator StartMessage()
    {
        startMessage.text = "Kill the incoming monsters, complete the tasks and escape.";
        yield return new WaitForSeconds(5.5f);
        startMessage.text = "";
    }
}
