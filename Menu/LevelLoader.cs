using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private Text progressText;

    public void Awake()
    {
        LoadLevel();
    }

    public void LoadLevel()
    {
        //StartCoroutine(LoadScene());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

   /* IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return new WaitForSeconds(1); 
        }
    }*/
}
