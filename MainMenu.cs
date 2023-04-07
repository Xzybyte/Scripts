using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject modeSelect;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject gameData;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Text musicVolumeText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Text soundVolumeText;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Text mouseSens;
    [SerializeField] private Slider mouseSlider;
    [SerializeField] private Text gamepadSens;
    [SerializeField] private Slider gamepadSlider;
    [SerializeField] private Toggle display;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite orangeSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite purpleSprite;
    [SerializeField] private Sprite blueHandleSprite;
    private Resolution[] resolutions;
    private bool fullScreen;
    private bool boot = true;
    private int mode = 1;

    private void Awake()
    {
        //gameData.GetComponent<GameData>().SaveGame();
        gameData.GetComponent<GameData>().LoadGame();
        resolutions = Screen.resolutions;
        fullScreen = Screen.fullScreen;
        if (!fullScreen)
        {
            display.isOn = false;
        }
        boot = false;
        gamepadSlider.value = PlayerPrefs.GetFloat("GamepadSensitivity", 2f);
        mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        for (int i = 0; i < transform.childCount; i++)
        {
            foreach (var obj in transform.GetChild(i).GetComponentsInChildren<Selectable>().Select(x => x.gameObject))
            {
                obj.AddComponent<Cancel>();
                obj.GetComponent<Cancel>().SetMenu(mainMenu);
                obj.GetComponent<Cancel>().SetModes(modeSelect);
                obj.GetComponent<Cancel>().SetOptions(options);
                obj.GetComponent<Cancel>().SetCharSelect(characterSelect);
                obj.GetComponent<Cancel>().SetControls(controls);
                obj.GetComponent<Cancel>().SetRedSprite(redSprite);
                obj.GetComponent<Cancel>().SetGreenSprite(greenSprite);
                obj.GetComponent<Cancel>().SetOrangeSprite(orangeSprite);
                obj.GetComponent<Cancel>().SetPurpleSprite(purpleSprite);
                obj.GetComponent<Cancel>().SetBlueHandleSprite(blueHandleSprite);
            }
        } 
    }

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
    }

    public void PlayGame()
    {
        mainMenu.SetActive(false);
        modeSelect.transform.GetChild(9).GetComponent<Button>().interactable = gameData.GetComponent<GameData>().HasDefeatedGame();
        modeSelect.SetActive(true);
    }

    public void PlayAsMale()
    {
        PlayerPrefs.SetString("Character", "male");
        Debug.Log(PlayerPrefs.GetString("Cutscene").Equals("Complete"));
        if (PlayerPrefs.GetString("Cutscene").Equals("Complete"))
        {
            SceneManager.LoadScene("Game_" + PlayerPrefs.GetString("Mode"));
            FindObjectOfType<AudioManager>().Stop("menutheme");
            FindObjectOfType<AudioManager>().Play("theme");
        } else
        {
            SceneManager.LoadScene("Cutscene");
        }
    }

    public void PlayAsFemale()
    {
        PlayerPrefs.SetString("Character", "female");
        if (PlayerPrefs.GetString("Cutscene").Equals("Complete"))
        {
            SceneManager.LoadScene("Game_" + PlayerPrefs.GetString("Mode"));
            FindObjectOfType<AudioManager>().Stop("menutheme");
            FindObjectOfType<AudioManager>().Play("theme");
        }
        else
        {
            SceneManager.LoadScene("Cutscene_2");
        }
    }

    public void SetNormal()
    {
        mode = 1;
        PlayerPrefs.SetString("Mode", "Normal");
    }

    public void SetHard()
    {
        mode = 2;
        PlayerPrefs.SetString("Mode", "Hard");
    }

    public void SetExpert()
    {
        mode = 3;
        PlayerPrefs.SetString("Mode", "Expert");
    }

    public void SetEndless()
    {
        mode = Random.Range(1, 4);
        PlayerPrefs.SetString("Mode", "Endless");
    }

    public void BackFromModeSelect()
    {
        mainMenu.SetActive(true);
        modeSelect.SetActive(false);
    }

    public void Options()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
    }

    public void BackFromOptions()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);  
    }

    public void Controls()
    {
        options.SetActive(false);
        controls.SetActive(true);
    }

    public void BackFromControls()
    {
        options.SetActive(true);
        controls.SetActive(false);
    }

    public void CharacterSelect()
    {
        modeSelect.SetActive(false);
        characterSelect.SetActive(true);
    }

    public void BackFromCharacterSelect()
    {
        modeSelect.transform.GetChild(9).GetComponent<Button>().interactable = gameData.GetComponent<GameData>().HasDefeatedGame();
        modeSelect.SetActive(true);
        characterSelect.SetActive(false);
    }

    public void PlayCredits()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
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

    public void SetMouseSensitivity(float mSens)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mSens);
        mouseSens.text = mSens.ToString("0.00") + "";
    }

    public void SetGamepadSensitivity(float gSens)
    {
        PlayerPrefs.SetFloat("GamepadSensitivity", gSens);
        gamepadSens.text = gSens.ToString("0.00") + "";
    }

}
