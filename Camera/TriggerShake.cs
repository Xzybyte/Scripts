using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerShake : MonoBehaviour
{
    // Start is called before the first frame update
    public void ShakeCamera()
    {
        FindObjectOfType<AudioManager>().Play("explosion");
        CameraShaker.Instance.ShakeOnce(0.5f, 1f, 0.1f, 2f);
    }

    public void ShakeCamera2()
    {
        FindObjectOfType<AudioManager>().Play("explosion");
        CameraShaker.Instance.ShakeOnce(0.5f, 1f, 0.1f, 3f);
    }

    public void PlayBoss()
    {
        FindObjectOfType<AudioManager>().Stop("cutscene");
        FindObjectOfType<AudioManager>().Play("bossmusic");
    }

    public void StopBoss()
    {
        FindObjectOfType<AudioManager>().Stop("bossmusic");
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Game_" + PlayerPrefs.GetString("Mode"));
        PlayerPrefs.SetString("Cutscene", "Complete");
    }

    public void EndGame()
    {
        FindObjectOfType<AudioManager>().Stop("cutscene");
        FindObjectOfType<AudioManager>().Play("menutheme");
        SceneManager.LoadScene("Menu");
    }
}
