using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private int health = 1000;
    private int currentHealth;
    private int oxygen = 100;
    private int currentOxygen;
    private float oxygenInterval;
    private float oxygenInterval_2 = 5;
    private int oxygenRemoval = 1;
    [SerializeField] private PlayerHealth healthbar;
    [SerializeField] private OxygenLevel oxygenbar;
    [SerializeField] private Animator animator;
    [SerializeField] private List<GameObject> guns;
    [SerializeField] private GameObject currentGun;
    [SerializeField] private GameObject firstGun;
    [SerializeField] private GameObject secondGun;
    [SerializeField] private Text coinText;
    [SerializeField] private Text redPotText;
    [SerializeField] private Text bluePotText;
    [SerializeField] private Text statusMessage;
    [SerializeField] private Text doorMessage;
    [SerializeField] private Text gainedPotText;
    [SerializeField] private Text gainedCoinText;
    [SerializeField] private Text damageMultiplierText;
    [SerializeField] private Text bulletSpeedText;
    [SerializeField] private Text shootingDelayText;
    [SerializeField] private GameObject lowOxygenWarning;
    [SerializeField] private GameObject doubleCoinsImage;
    [SerializeField] private GameObject damageBonusImage;
    [SerializeField] private GameObject instantKillImage;
    [SerializeField] private GameObject movementSpeedImage;
    [SerializeField] private Text mouseSens;
    [SerializeField] private Slider mouseSlider;
    [SerializeField] private Text gamepadSens;
    [SerializeField] private Slider gamepadSlider;

    [SerializeField] private GameObject taskImage;
    [SerializeField] private Text taskText;
    [SerializeField] private GameObject taskInfo;
    private Keyboard keyboard;
    private Gamepad gamepad;
    private Mouse mouse;
    private PauseMenu pauseMenu;
    private bool taskBarOpen = false;

    //private Text statusMessage;

    [SerializeField] private Sprite[] sprite;

    [SerializeField] private Image firstWeaponImage;
    [SerializeField] private Image secondWeaponImage;

    private Coroutine statusCoroutine;
    private Coroutine doorCoroutine;
    private Coroutine gainedCoinCoroutine;
    private Coroutine taskCoroutine;

    private Coroutine doubleCoins;
    private bool doubleCoinsActive;
    private float doubleCoinsTimer = 100;
    private Coroutine damageBonus;
    private bool damageBonusActive;
    private float damageBonusTimer = 80;
    private Coroutine instantKill;
    private bool instantKillActive;
    private float instantKillTimer = 45;
    private Coroutine movementSpeed;
    private bool movementSpeedActive;
    private float movementSpeedTimer = 120;

    private Vector2 sensitivity = new Vector2(50, 50);
    private Vector2 mouseSensitivity = new Vector2(2f, 2f);

    private int iterations = 0;
    private bool nukeRunning = false;
    private bool nuked = false;

    private decimal damageMultiplier = 0;

    private decimal bulletSpeed = 18m;

    private decimal shootingDelay = 0.4m;

    private bool isDead = false;
    private bool oxygenWarning = false;
    private int gunIndex = 0;
    private int coins = 0;
    private int redPotions = 0;
    private int bluePotions = 0;
    private bool isMale;

    private ColorGrading cGrading;
    private Tasks task;
    private String difficulty = "";

    public static bool testing = false;

    private void Awake()
    {
        /* 0 = Ball (0 image, 0)
         * 1 = Blast (20 image, 4)
         * 2 = Disk (64 image, 8)
         * 3 = Moon (52 image, 7)
         * 4 = Magatma (4 image, 1)
         * 5 = Shuriken (44 image, 5)
         * 6 = Slash (48 image, 6)
         * 7 = Snake (16 image, 3)
         * 8 = Star (8 image, 2)
         * 9 = Arrow (Part2_92, 9)
         * 10 = Bomb (Part2_76, 10)
         */
        //float[] bulletForces = { 18, 19.5f, 20, 20.5f, 21, 21.5f, 22, 22.5f, 23, 24.5f, 25 };
        //float[] fireDelays = { 0.4f, 0.38f, 0.36f, 0.34f, 0.32f, 0.3f, 0.28f, 0.26f, 0.24f, 0.20f, 0.16f};
        isMale = PlayerPrefs.GetString("Character").Equals("male");
        difficulty = PlayerPrefs.GetString("Mode");
        gamepadSlider.value = PlayerPrefs.GetFloat("GamepadSensitivity", 2f);
        mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);

        keyboard = InputSystem.GetDevice<Keyboard>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        mouse = InputSystem.GetDevice<Mouse>();
        Camera.main.GetComponent<PostProcessVolume>().profile.TryGetSettings(out cGrading);
        currentGun = Instantiate(guns[0], this.transform);
        firstGun = guns[0];

        task = new Tasks(difficulty, taskInfo, this);

        if (!difficulty.Equals("Endless"))
        {
            task.AutoStartTask();
        }

        if (testing)
        {
            UpdateGun(8);
            IncreaseCoins(40000, false);
            IncreaseBluePotions(100);
            IncreaseRedPotions(99);
            damageMultiplier = 2.9m;
            bulletSpeed = 29.5m;
            shootingDelay = 0.22m;
        }

        pauseMenu = GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>();

        currentHealth = health;
        healthbar.SetMaxHealth(currentHealth);
        currentOxygen = oxygen;
        oxygenbar.SetMaxOxygen(currentOxygen);
    }

    private void Update()
    {
        if (pauseMenu.IsPaused())
        {
            return;
        }
        if (isDead)
        {
            return;
        }
        if (taskBarOpen)
        {
            if (!GetTaskSystem().HasTask())
            {
                taskText.text = "";
                taskBarOpen = false;
                taskImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1162, 194);
            }
        }
        if (keyboard != null && keyboard.qKey.wasPressedThisFrame || gamepad != null && gamepad.buttonEast.wasPressedThisFrame)
        {
            HealPlayer();
        }
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame || gamepad != null && gamepad.buttonWest.wasPressedThisFrame)
        {
            RestoreOxygen();
        }
        if (keyboard != null && keyboard.tKey.wasPressedThisFrame || gamepad != null && gamepad.buttonNorth.wasPressedThisFrame)
        {
            ShowTask(true);
        } else if (keyboard != null && keyboard.tKey.wasReleasedThisFrame || gamepad != null && gamepad.buttonNorth.wasReleasedThisFrame)
        {
            ShowTask(false);
        }
        if (mouse != null)
        {
            ChangeWeapon(mouse.scroll.ReadValue().y);
        }

        if (gamepad != null)
        {
            if (gamepad.rightTrigger.wasPressedThisFrame)
            {
                ChangeWeapon(-1f);
            } else if (gamepad.leftTrigger.wasPressedThisFrame)
            {
                ChangeWeapon(1f);
            }
        }
        oxygenInterval += Time.deltaTime;
        if (oxygenInterval >= oxygenInterval_2)
        {
            TakeOxygen(currentOxygen);
            oxygenInterval = 0;
        }
        if (doubleCoinsActive)
        {
            doubleCoinsTimer -= Time.deltaTime;
            if (doubleCoinsTimer >= 0)
            {
                int seconds = Mathf.RoundToInt(doubleCoinsTimer);
                doubleCoinsImage.transform.GetChild(0).GetComponent<Text>().text = seconds.ToString();
            }
        }
        if (damageBonusActive)
        {
            damageBonusTimer -= Time.deltaTime;
            if (damageBonusTimer >= 0)
            {
                int seconds = Mathf.RoundToInt(damageBonusTimer);
                damageBonusImage.transform.GetChild(0).GetComponent<Text>().text = seconds.ToString();
            }
        }
        if (instantKillActive)
        {
            instantKillTimer -= Time.deltaTime;
            if (instantKillTimer >= 0)
            {
                int seconds = Mathf.RoundToInt(instantKillTimer);
                instantKillImage.transform.GetChild(0).GetComponent<Text>().text = seconds.ToString();
            }
        }
        if (movementSpeedActive)
        {
            movementSpeedTimer -= Time.deltaTime;
            if (movementSpeedTimer >= 0)
            {
                int seconds = Mathf.RoundToInt(movementSpeedTimer);
                movementSpeedImage.transform.GetChild(0).GetComponent<Text>().text = seconds.ToString();
            }
        }
    }

    private void ChangeWeapon(float scroll)
    {
        float mouseDelta = scroll;
        if (mouseDelta == -120 || mouseDelta == -1)
        {
            if (secondGun != null && currentGun.name.Contains(firstGun.name))
            {
                Destroy(currentGun);
                currentGun = Instantiate(secondGun, this.transform);
                GetComponent<Shooting>().updateGun(currentGun, false, getNameByGun(gunIndex));
                secondWeaponImage.color = new Color32(40, 44, 60, 255);
                firstWeaponImage.color = new Color32(94, 113, 142, 255);
            }
        }
        else if (mouseDelta == 120 || mouseDelta == 1)
        {
            if (secondGun != null && currentGun.name.Contains(secondGun.name))
            {
                Destroy(currentGun);
                currentGun = Instantiate(firstGun, this.transform);
                GetComponent<Shooting>().updateGun(currentGun, true, getNameByGun(0));
                firstWeaponImage.color = new Color32(40, 44, 60, 255);
                secondWeaponImage.color = new Color32(94, 113, 142, 255);
            }
        }
    }

    private void ShowTask(bool show)
    {
        if (GetTaskSystem().HasTask())
        {
            if (show)
            {
                taskInfo.GetComponent<Text>().text = "";
                taskInfo.GetComponent<Animator>().enabled = false;
                taskImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-709, 194);
                taskText.text = "Task " + (GetTaskSystem().GetTaskCompleted() + 1) + ":\r\n \r\n" + GetTaskSystem().GetTaskDescription() + ".";
                taskBarOpen = true;
            }
            else
            {
                taskText.text = "";
                taskImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1162, 194);
                taskBarOpen = false;
            }
        }
    }

    public void UpdateGun(int index)
    {
        /*0 = Ball(0 image, 0)
         * 1 = Blast(20 image, 4)
         * 2 = Disk(64 image, 8)
         * 3 = Moon(52 image, 7)
         * 4 = Magatma(4 image, 1)
         * 5 = Shuriken(44 image, 5)
         * 6 = Slash(48 image, 6)
         * 7 = Snake(16 image, 3)
         * 8 = Star(8 image, 2)
         * 9 = Arrow(Part2_92, 9)
         * 10 = Bomb(Part2_76, 10)
         */
        FindObjectOfType<AudioManager>().Play("weapon");
        int[] indexes = {0, 4, 8, 7, 1, 5, 6, 3, 2, 9, 10};
        int[] gunIndexes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        for (int i = 0; i < indexes.Length; i++)
        {
            if (indexes[i] == index)
            {
                GameObject newGun = guns[gunIndexes[i]];
                gunIndex = indexes[i];
                bool secondGunActive = secondGun != null && currentGun.name.Contains(secondGun.name);
                if (secondGunActive)
                {
                    Destroy(currentGun);
                    currentGun = Instantiate(newGun, this.transform);
                    GetComponent<Shooting>().updateGun(currentGun, false, getNameByGun(gunIndex));
                }
                secondGun = newGun;
                secondWeaponImage.transform.GetChild(1).GetComponent<Image>().sprite = sprite[indexes[i]];
                GetComponent<Shooting>().resetAmmo();
                Color32 imageColour = secondWeaponImage.transform.GetChild(1).GetComponent<Image>().color;
                secondWeaponImage.transform.GetChild(1).GetComponent<Image>().color = new Color32(imageColour.r, imageColour.g, imageColour.b, 255);

                break;
            }
        }
    }

    public void GiveBuff(int index)
    {
        FindObjectOfType<AudioManager>().Play("buff");
        if (index == 0)
        {
            doubleCoinsTimer = 100;
            doubleCoinsImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            doubleCoinsActive = true;
            DoubleCoinsBonus();
        } else if (index == 1)
        {
            damageBonusTimer = 80;
            damageBonusImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            damageBonusActive = true;
            DamageBonus();
        } else if (index == 2)
        {
            instantKillTimer = 45;
            instantKillImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            instantKillActive = true;
            InstantKillBonus();
        } else if (index == 3)
        {
            StatusMessage("Your ammo has been replenished.", 3);
            GetComponent<Shooting>().resetAmmo();
        } else if (index == 4)
        {
            movementSpeedTimer = 120;
            movementSpeedImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            movementSpeedActive = true;
            MovementSpeedBonus();
        }
    }

    public void TakeOxygen(int currentOxygen)
    {
        if (isDead)
        {
            return;
        }
        currentOxygen -= oxygenRemoval;
        this.currentOxygen = currentOxygen;
        oxygenbar.SetOxygen(this.currentOxygen);
        if (this.currentOxygen <= 30 && !oxygenWarning)
        {
            oxygenWarning = true;
            lowOxygenWarning.GetComponent<Text>().text = "Low Oxygen Warning!";
            lowOxygenWarning.GetComponent<Animator>().enabled = true;
        }
        if (currentOxygen <= 0)
        {
            oxygenbar.SetOxygen(0);
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }
        FindObjectOfType<AudioManager>().Play("playerdamaged");
        CameraShaker.Instance.ShakeOnce(2f, 1f, 0.1f, 1f);
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
        UpdateSaturation();
        //Debug.Log(currentHealth);

        animator.SetTrigger("Damaged");
        float time = 0f;
        string clipName = "Damaged_Animation";
        if (!isMale)
        {
            clipName = "Damaged_Animation_Player_2";
        }

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == clipName)
            {
                time = clips[i].length;
                break;
            }
        }

        StartCoroutine(HitPlayer(time));

        if (currentHealth <= 0)
        {
            healthbar.SetHealth(0);
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        string clipName = "Death_Animation";
        if (!isMale)
        {
            clipName = "Death_Animation_Player_2";
        }

        animator.Play(clipName);

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
        StartCoroutine(ResetPlayer(time));
    }

    private void UpdateSaturation()
    {
        if (currentHealth >= 401)
        {
            cGrading.saturation.value = 0;
        } else if (currentHealth <= 400 && currentHealth >= 301)
        {
            cGrading.saturation.value = -25;
        } else if (currentHealth <= 300 && currentHealth >= 201)
        {
            cGrading.saturation.value = -50;
        } else if (currentHealth <= 200 && currentHealth >= 101)
        {
            cGrading.saturation.value = -75;
        } else
        {
            cGrading.saturation.value = -100;
        }
    }

    private IEnumerator ResetPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        Time.timeScale = 0;
        GameObject.FindGameObjectWithTag("DeathScreen").GetComponent<DeathScreen>().StartDeathScreen();
    }

    private IEnumerator HitPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        animator.ResetTrigger("Damaged");
    }

    public bool getDead()
    {
        return isDead;
    }

    public GameObject getCurrentGun()
    {
        return currentGun;
    }

    public int getDamageByGun()
    {
        string[] guns = {"Ball", "Magatma", "Star", "Snake", "Blast", "Shuriken", "Slash", "Half Moon", "Disk", "Arrow", "Bomb"};
        int[] damages = {50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600};

        int damage = 0;

        for (int i = 0; i < guns.Length; i++)
        {
            if (currentGun.name.Contains(guns[i]))
            {
                damage = damages[i];
                break;
            }
        }
        if (damageBonusActive)
        {
            damage += 250;
        }
        damage += (int) (damage * damageMultiplier);
        return damage;
    }

    public string getNameByGun(int index) 
    {
        /* 0 = Ball (0 image, 0)
         * 1 = Blast (20 image, 4)
         * 2 = Disk (64 image, 8)
         * 3 = Moon (52 image, 7)
         * 4 = Magatma (4 image, 1)
         * 5 = Shuriken (44 image, 5)
         * 6 = Slash (48 image, 6)
         * 7 = Snake (16 image, 3)
         * 8 = Star (8 image, 2)
         */
        string[] guns = { "Ball", "Magatma", "Star", "Snake", "Blast", "Shuriken", "Slash", "Half Moon", "Disk", "Arrow", "Bomb" };
        return guns[index];
    }

    public string getNameByBuff(int index)
    {
        string[] buffs = { "Double Coins" , "Damage Bonus", "Instant Kill", "Maximum Ammo", "Movement Buff" };
        return buffs[index];
    }

    public void IncreaseCoins(int coins, bool drop)
    {
        if (doubleCoinsActive && drop)
        {
            coins *= 2;
        }
        this.coins += coins;
        coinText.text = "" + System.String.Format("{0:n0}", this.coins);
        GainedCoinMessage((coins > 0 ? "+":"") +  ""  + coins, 3);
    }

    public int GetCoins()
    {
        return coins;
    }

    public void IncreaseRedPotions(int redP)
    {
        this.redPotions += redP;
        redPotText.text = "" + System.String.Format("{0:n0}", this.redPotions);
    }

    public int GetRedPotions()
    {
        return redPotions;
    }

    public void HealPlayer()
    {
        if (GetRedPotions() >= 1)
        {
            if (currentHealth != health)
            {
                int heal = 100;
                if (heal + currentHealth > health)
                {
                    currentHealth = health;
                    healthbar.SetHealth(currentHealth);
                    UpdateSaturation();
                    IncreaseRedPotions(-1);
                    return;
                }
                currentHealth += heal;
                healthbar.SetHealth(currentHealth);
                UpdateSaturation();
                IncreaseRedPotions(-1);
            }
            else
            {
                StatusMessage("You are already at Max HP!", 3);
            }
        } else
        {
            StatusMessage("You do not have any red potions to use.", 3);
        }
    }

    public void IncreaseBluePotions(int blueP)
    {
        this.bluePotions += blueP;
        bluePotText.text = "" + System.String.Format("{0:n0}", this.bluePotions);
    }

    public int GetBluePotions()
    {
        return bluePotions;
    }

    public void RestoreOxygen()
    {
        if (GetBluePotions() >= 1)
        {
            if (currentOxygen != oxygen)
            {
                int heal = 10;
                if (heal + currentOxygen > oxygen)
                {
                    currentOxygen = oxygen;
                    oxygenbar.SetOxygen(currentOxygen);
                    IncreaseBluePotions(-1);
                    if (oxygenWarning)
                    {
                        lowOxygenWarning.GetComponent<Text>().text = "";
                        lowOxygenWarning.GetComponent<Animator>().enabled = false;
                        oxygenWarning = false;
                    }
                    return;
                }
                currentOxygen += heal;
                oxygenbar.SetOxygen(currentOxygen);
                IncreaseBluePotions(-1);
                if (oxygenWarning && currentOxygen > 30)
                {
                    lowOxygenWarning.GetComponent<Text>().text = "";
                    lowOxygenWarning.GetComponent<Animator>().enabled = false;
                    oxygenWarning = false;
                }
            }
            else
            {
                StatusMessage("You are already at Max Oxygen!", 3);
            }
        }
        else
        {
            StatusMessage("You do not have any blue potions to use.", 3);
        }
    }

    public void StatusMessage(string text, int seconds)
    {
        if (statusCoroutine != null)
        {
            StopCoroutine(statusCoroutine);
        }
        seconds += 3;
        statusMessage.text = text;
        statusCoroutine = StartCoroutine(StatusMessageClear(seconds));
    }

    private IEnumerator StatusMessageClear(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        statusMessage.text = "";
    }

    public void TaskMessage(string text, int seconds)
    {
        if (taskCoroutine != null)
        {
            StopCoroutine(taskCoroutine);
        }
        Color32 taskColor = taskInfo.GetComponent<Text>().color;
        taskInfo.GetComponent<Text>().color = new Color32(taskColor.r, taskColor.g, taskColor.b, 255);
        taskInfo.GetComponent<Text>().text = text;
        taskCoroutine = StartCoroutine(TaskMessageClear(seconds));
    }

    private IEnumerator TaskMessageClear(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        taskInfo.GetComponent<Text>().text = "";
    }

    public void DoorMessage(string text)
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }
        doorMessage.text = text;
        doorCoroutine = StartCoroutine(DoorMessageClear());
    }

    public void ClearMessage()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }
        doorMessage.text = "";
    }

    private IEnumerator DoorMessageClear()
    {
        yield return new WaitForSeconds(6);
        doorMessage.text = "";
    }

    private void GainedCoinMessage(string text, int seconds)
    {
        if (gainedCoinCoroutine != null)
        {
            StopCoroutine(gainedCoinCoroutine);
        }
        gainedCoinText.text = text;
        gainedCoinCoroutine = StartCoroutine(GainedMessageClear(seconds));
    }

    private IEnumerator GainedMessageClear(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        gainedCoinText.text = "";
    }

    private void DoubleCoinsBonus()
    {
        if (doubleCoins != null)
        {
            StopCoroutine(doubleCoins);
        }
        doubleCoins = StartCoroutine(EndDoubleCoinBonus());
    }

    private IEnumerator EndDoubleCoinBonus()
    {
        yield return new WaitForSeconds(100);
        doubleCoinsActive = false;
        doubleCoinsImage.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        doubleCoinsImage.transform.GetChild(0).GetComponent<Text>().text = "";
    }

    private void DamageBonus()
    {
        if (damageBonus != null)
        {
            StopCoroutine(damageBonus);
        }
        damageBonus = StartCoroutine(EndDamageBonus());
    }

    private IEnumerator EndDamageBonus()
    {
        yield return new WaitForSeconds(80);
        damageBonusActive = false;
        damageBonusImage.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        damageBonusImage.transform.GetChild(0).GetComponent<Text>().text = "";
    }

    private void InstantKillBonus()
    {
        if (instantKill != null)
        {
            StopCoroutine(instantKill);
        }
        instantKill = StartCoroutine(EndInstantKill());
    }

    private IEnumerator EndInstantKill()
    {
        yield return new WaitForSeconds(45);
        instantKillActive = false;
        instantKillImage.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        instantKillImage.transform.GetChild(0).GetComponent<Text>().text = "";
    }

    private void MovementSpeedBonus()
    {
        if (movementSpeed != null)
        {
            StopCoroutine(movementSpeed);
        }
        movementSpeed = StartCoroutine(EndMovementSpeedBonus());
    }

    private IEnumerator EndMovementSpeedBonus()
    {
        yield return new WaitForSeconds(120);
        movementSpeedActive = false;
        movementSpeedImage.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        movementSpeedImage.transform.GetChild(0).GetComponent<Text>().text = "";
    }


    public bool GetInstantKill()
    {
        return instantKillActive;
    }

    public bool GetMovementSpeed()
    {
        return movementSpeedActive;
    }

    public Tasks GetTaskSystem()
    {
        return task;
    }

    public void SetOxygenInterval_2(float interv, int removal)
    {
        oxygenInterval_2 = interv;
        oxygenRemoval = removal;
    }

    public void DamageMultiplier(int cost)
    {
        if (damageMultiplier >= 3m)
        {
            StatusMessage("You cannot increase your damage multiplier any further.", 3);
            return;
        }
        FindObjectOfType<AudioManager>().Play("mods");
        IncreaseCoins(-cost, false);
        damageMultiplier += 0.1m;
        damageMultiplierText.text = "DMG MULTIPLIER: " + damageMultiplier + "x";
        StatusMessage("You have increased your damage multiplier by 0.1x", 3);
        if (damageMultiplier == 3m)
        {
            damageMultiplierText.text = "DMG MULTIPLIER: " + damageMultiplier + "x*";
            return;
        }
    }

    public decimal GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public void BulletSpeed(int cost)
    {
        if (bulletSpeed >= 30m)
        {
            StatusMessage("You cannot increase your fire speed any further.", 3);
            return;
        }
        FindObjectOfType<AudioManager>().Play("mods");
        IncreaseCoins(-cost, false);
        bulletSpeed += 0.5m;
        bulletSpeedText.text = "FIRE SPEED: " + bulletSpeed + "f";
        StatusMessage("You have increased your fire speed by 0.5p", 3);
        if (bulletSpeed == 30m)
        {
            bulletSpeedText.text = "FIRE SPEED: " + bulletSpeed + "f*";
            return;
        }
    }

    public decimal GetBulletSpeed()
    {
        return bulletSpeed;
    }

    public void ShotDelay(int cost)
    {
        if (shootingDelay <= 0.20m)
        {
            StatusMessage("You cannot decrease your shooting delay any further.", 3);
            return;
        }
        FindObjectOfType<AudioManager>().Play("mods");
        IncreaseCoins(-cost, false);
        shootingDelay -= 0.02m;
        shootingDelayText.text = "SHOOTING DELAY: " + shootingDelay + "s";
        StatusMessage("You have decreased your shooting delay by 0.02s", 3);
        if (shootingDelay == 0.20m)
        {
            shootingDelayText.text = "SHOOTING DELAY: " + shootingDelay + "s*";
            return;
        }
    }

    public decimal GetShootingDelay()
    {
        return shootingDelay;
    }

    public void StartNuke()
    {
        if (!nukeRunning)
        {
            nukeRunning = true;
            StartCoroutine(WhiteScreen());
        }
    }

    private IEnumerator WhiteScreen()
    {
        //CameraShaker.Instance.ShakeOnce(2f, 1f, 0.1f, 1f);
        while (true)
        {
            if (!nuked)
            {
                yield return new WaitForSeconds(0.35f);
                iterations++;
                if (iterations <= 4)
                {
                    cGrading.contrast.value -= 25;
                }
                else
                {
                    nuked = true;
                    iterations = 0;
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        Enemy enemy = enemies[i].GetComponent<Enemy>();
                        if (!enemy.IsBoss() && !enemy.getDead())
                        {
                            enemy.Die();
                        }
                    }
                    FindObjectOfType<AudioManager>().Play("nuke");
                }
            } else
            {
                yield return new WaitForSeconds(0.25f);
                iterations++;
                if (iterations <= 4)
                {
                    cGrading.contrast.value += 25;
                }
                else
                {
                    nuked = false;
                    nukeRunning = false;
                    iterations = 0;
                    break;
                }
            }
        }
    }

    public Vector2 GetMouseSensitivity()
    {
        return mouseSensitivity;
    }

    public Vector2 GetGamepadSensitivity()
    {
        return sensitivity;
    }

    public void SetMouseSensitivity(float mSens)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mSens);
        mouseSens.text = mSens.ToString("0.00") + "";
        mouseSensitivity = new Vector2(mSens, mSens);
    }

    public void SetGamepadSensitivity(float gSens)
    {
        PlayerPrefs.SetFloat("GamepadSensitivity", gSens);
        gamepadSens.text = gSens.ToString("0.00") + "";
        sensitivity = new Vector2(gSens * 25f, gSens * 25f);
    }
}
