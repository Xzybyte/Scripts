using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject game;
    [SerializeField] private GameObject over;
    [SerializeField] private GameObject YesNo;
    private float gameTime;
    private float overTime;

    public void StartDeathScreen()
    {        
        gameTime = SetAnimationTimes(game.GetComponent<Animator>(), "Game");
        overTime = SetAnimationTimes(over.GetComponent<Animator>(), "Over");

        StartCoroutine(DeathScreenShow());
    }

    private IEnumerator DeathScreenShow()
    {
        game.SetActive(true);
        //game.GetComponent<Animator>().Play("Game");
        yield return new WaitForSecondsRealtime(gameTime);
        over.SetActive(true);
        //over.GetComponent<Animator>().Play("Over");
        yield return new WaitForSecondsRealtime(overTime);
        // Enable replay button
        YesNo.SetActive(true);

    }

    private float SetAnimationTimes(Animator animator, string clipName)
    {
        float time = 0f;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == clipName)
            {
                time = clips[i].length;
                break;
            }
        }
        return time;
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DontRetry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
