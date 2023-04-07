using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Text musicVolumeText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Text soundVolumeText;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject reallyQuit;
    [SerializeField] private Toggle display;
    [SerializeField] private Sprite blueSliderHandle;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private bool pause;
    private Resolution[] resolutions;
    private bool fullScreen;
    private bool inControls;
    private bool boot = true;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        fullScreen = Screen.fullScreen;
        if (!fullScreen)
        {
            display.isOn = false;
        }
        boot = false;
        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.getDead())
        {
            return;
        }
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame || gamepad != null && gamepad.startButton.wasPressedThisFrame)
        {
            if (!inControls)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pause = !pause;
        if (pause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            pauseMenu.SetActive(false);
            ResetHandles();
            Time.timeScale = 1;
        }
    }

    private void ResetHandles()
    {
        pauseMenu.transform.GetChild(5).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueSliderHandle;
        pauseMenu.transform.GetChild(6).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueSliderHandle;
        pauseMenu.transform.GetChild(7).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueSliderHandle;
        pauseMenu.transform.GetChild(8).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueSliderHandle;
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
        FindObjectOfType<AudioManager>().Stop("theme");
        FindObjectOfType<AudioManager>().Play("menutheme");
    }

    public void DoNotQuitGame()
    {
        PauseGame();
    }
    public bool IsPaused()
    {
        return pause;
    }

    public void ChangeDisplay()
    {
        if (boot)
        {
            return;
        }
        if (fullScreen)
        {
            Screen.SetResolution(1280, 720, false);
            fullScreen = false;
        }
        else
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
            fullScreen = true;
        }
    }

    public void SetMusicVolume(float mVolume)
    {
        audioMixer.SetFloat("musicvolume", Mathf.Log10(mVolume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", mVolume);
        musicVolumeText.text = Mathf.Round(mVolume * 100) + "%";
    }

    public void SetSoundVolume(float sVolume)
    {
        audioMixer.SetFloat("soundvolume", Mathf.Log10(sVolume) * 20);
        PlayerPrefs.SetFloat("SoundVolume", sVolume);
        soundVolumeText.text = Mathf.Round(sVolume * 100) + "%";
    }

    public void Controls()
    {
        inControls = true;
        options.SetActive(false);
        controls.SetActive(true);
    }

    public void BackFromControls()
    {
        inControls = false;
        options.SetActive(true);
        controls.SetActive(false);
    }

    public void ReallyQuit()
    {
        reallyQuit.SetActive(true);
        options.SetActive(false);
    }

    public void BackFromReallyQuit()
    {
        options.SetActive(true);
        reallyQuit.SetActive(false);
    }

    public void SetInControls()
    {
        StartCoroutine(DelayEsc());
    }

    private IEnumerator DelayEsc()
    {
        yield return new WaitForSecondsRealtime(1f);
        this.inControls = false;
    }

}
