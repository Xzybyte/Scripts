using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyRespawn : MonoBehaviour
{
    private Transform player;
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [SerializeField] private GameObject malePrefab;
    [SerializeField] private GameObject femalePrefab;
    [SerializeField] private Animator fadeInDarkness;

    [SerializeField] private List<GameObject> enemies;
    private List<Transform> spawnPositions = new List<Transform>();
    private List<Transform> doorPositions = new List<Transform>();
    private float respawnTimer = 3f;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject bossCanvas;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject spectralSword;
    [SerializeField] private GameObject hog;
    [SerializeField] private Text waveText;
    [SerializeField] private Text currentWaveText;
    [SerializeField] private Text mobToNextWaveText;
    [SerializeField] private GameObject chestRespawner;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject redPot;
    [SerializeField] private GameObject bluePot;
    [SerializeField] private GameObject instantKill;
    [SerializeField] private GameObject darknessCanvas;
    [SerializeField] private GameObject compass;
    [SerializeField] private GameObject lever;
    [SerializeField] private GameObject gameData;
    private GameObject leverInstance;
    private GameObject roomNumbers;
    private Player p;
    private Transform bossPosition;
    private int currentWave = 0;
    private int maxMonsters = 5;
    private int currentMonsters = 0;
    private int spawnedMonsters = 0;
    private int healthIncrease = 0;
    private bool startRoutine = true;

    private int startRange = 0;
    private int endRange = 0;
    private int minCoin = 0;
    private int maxCoin = 0;
    private float movementSpeed;
    private int maxDamage = 0;
    private int knockback = 0;
    private string difficulty = "";
    private bool darkness;
    private int trapMonstersKilled = 0;
    private int trapMonstersNeeded = 5;
    private List<int> doorsUnlocked = new List<int>();
    private bool bossSpawned;
    private bool male;

    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        roomNumbers = GameObject.FindGameObjectWithTag("RoomNumbers");

        male = PlayerPrefs.GetString("Character").Equals("male");
        if (male)
        {
            player = player1;
        } else
        {
            player = player2;
        }

        if (Player.testing)
        {
            currentWave = 44;
        }

        p = player.GetComponent<Player>();
        difficulty = PlayerPrefs.GetString("Mode");
        waveText.text = "";
        PopulateSpawnPositions();
        DisplayWaveInformation(false, false);
        StartGame(); // Start game
    }

    void StartGame()
    {
        if (difficulty.Equals("Expert"))
        {
            movementSpeed = 2;
        }
        if (spawnPositions.Count > 0 && enemies.Count > 0)  // Check if enemy and spawn positions are over 0
        {
            UpdateWaveInformation();
            StartWaves();
        }
    }

    private void UpdateWaveInformation()
    {
        //movementSpeed = 1;
        for (int i = 0; i < currentWave; i++)
        {
            if (respawnTimer >= 0.8f)
            {
                respawnTimer -= 0.1f;
            }
            maxMonsters += 2;
        }
    }

    private void PopulateSpawnPositions()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Room 1")
            {
                spawnPositions.Add(transform.GetChild(i));
            }
        }
    }

    private void PopulateRoomPositions()
    {
        for (int i = 0; i < roomNumbers.transform.childCount; i++)
        {
            if (roomNumbers.transform.GetChild(i).name == "Room 1")
            {
                spawnPositions.Add(transform.GetChild(i));
            }
        }
    }

    private void UpdateSpawnPositions()
    {
        for (int i = 0; i < doorsUnlocked.Count; i++)
        {
            for (int g = 0; g < transform.childCount; g++)
            {
                if (!spawnPositions.Contains(transform.GetChild(g)))
                { 
                    if (transform.GetChild(g).name == ("Room " + doorsUnlocked[i]))
                    {
                        spawnPositions.Add(transform.GetChild(g));
                    }
                }
            }
        }
    }

    private void UpdateDoorPositions()
    {
        for (int i = 0; i < doorsUnlocked.Count; i++)
        {
            for (int g = 0; g < roomNumbers.transform.childCount; g++)
            {
                if (!doorPositions.Contains(roomNumbers.transform.GetChild(g)))
                {
                    if (roomNumbers.transform.GetChild(g).name == ("Room " + doorsUnlocked[i]))
                    {
                        doorPositions.Add(roomNumbers.transform.GetChild(g));
                    }
                }
            }
        }
    }

    public void StartWaves()
    {
        //Debug.Log(difficulty);
        if (currentMonsters == 0)
        {
            spawnedMonsters = 0;
            SpawnChest();
            if (!difficulty.Equals("Endless"))
            {
                HandleTaskSystem();
            }
            if (bossSpawned)
            {
                return;
            }

            coroutine = StartCoroutine(WaveInformation(8f));
        }
    }

    public void HandleTaskSystem()
    {
        if (!p.GetTaskSystem().HasTask())
        {
            if (p.GetTaskSystem().GetNextTaskWave() == currentWave)
            { 
                p.GetTaskSystem().RandomizeTask();
                if (p.GetTaskSystem().GetCurrentTask().Equals("Protect"))
                {
                    SpawnHog();
                } else if (p.GetTaskSystem().GetCurrentTask().Equals("Kill"))
                {
                    SpawnBoss();
                } else if (p.GetTaskSystem().GetCurrentTask().Equals("Light"))
                {
                    Darkness(true);
                } else if (p.GetTaskSystem().GetCurrentTask().Equals("Arrow"))
                {
                    if (difficulty.Equals("Normal"))
                    {
                        trapMonstersNeeded = 5;
                    } else if (difficulty.Equals("Hard")) {
                        trapMonstersKilled = 10;
                    } else if (difficulty.Equals("Expert"))
                    {
                        trapMonstersKilled = 15;
                    }
                } else if (p.GetTaskSystem().GetCurrentTask().Equals("Fire"))
                {
                    if (difficulty.Equals("Normal"))
                    {
                        trapMonstersNeeded = 10;
                    }
                    else if (difficulty.Equals("Hard"))
                    {
                        trapMonstersKilled = 15;
                    }
                    else if (difficulty.Equals("Expert"))
                    {
                        trapMonstersKilled = 20;
                    }
                } else if (p.GetTaskSystem().GetCurrentTask().Equals("Poison"))
                {
                    if (difficulty.Equals("Normal"))
                    {
                        trapMonstersNeeded = 15;
                    }
                    else if (difficulty.Equals("Hard"))
                    {
                        trapMonstersKilled = 20;
                    }
                    else if (difficulty.Equals("Expert"))
                    {
                        trapMonstersKilled = 25;
                    }
                }
            }
        } else
        {
            if (p.GetTaskSystem().GetWaveRequired() == currentWave)
            {
                if (p.GetTaskSystem().GetCurrentTask().Equals("Protect"))
                {
                    p.GetTaskSystem().CompleteTask(currentWave);
                    GameObject hog = GameObject.FindGameObjectWithTag("HedgeHog");
                    if (hog != null)
                    {
                        Destroy(hog);
                    }
                }
            }
        }
    }

    public void KillMonsterWithTrap()
    {
        if (p.GetTaskSystem().GetCurrentTask().Equals("Arrow") || p.GetTaskSystem().GetCurrentTask().Equals("Fire") || p.GetTaskSystem().GetCurrentTask().Equals("Poison"))
        {
            trapMonstersKilled++;
            if (trapMonstersKilled >= trapMonstersNeeded)
            {
                p.GetTaskSystem().CompleteTask(currentWave);
                trapMonstersKilled = 0;
            }
        }
    }

    public void FinishStoryMode(Transform trans)
    {
        gameData.GetComponent<GameData>().SaveGame();
        if (male)
        {
            Instantiate(femalePrefab, trans.position, Quaternion.identity);
        } else
        {
            Instantiate(malePrefab, trans.position, Quaternion.identity);
        }
        StartCoroutine(EndScene());
    }

    private IEnumerator EndScene()
    {
        while (true)
        {
            int count = 0;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                count++;
            }
            if (count == 0)
            {
                break;
            }
            yield return null;
        }
        fadeInDarkness.SetBool("end", true);
        yield return new WaitForSeconds(4.5f);
        string scene = male ? "EndScene" : "EndScene_2";
        FindObjectOfType<AudioManager>().Stop("theme");
        FindObjectOfType<AudioManager>().Play("cutscene");
        SceneManager.LoadScene(scene);
    }
    
    public void SpawnChest()
    {
        GameObject chest = GameObject.FindGameObjectWithTag("Chest");
        if (chest == null)
        {
            if (currentWave >= 2)
            {
                if (DoorSize() >= 1)
                {
                    int randomChest = Random.Range(0, 100);
                    if (randomChest <= (40 + (doorsUnlocked.Count * 2)))
                    {
                        chestRespawner.GetComponent<ChestRespawner>().UpdateSpawnPositions(doorsUnlocked);
                        chestRespawner.GetComponent<ChestRespawner>().SpawnChest();
                    }
                }
            }
        }
    }

    public void Darkness(bool enable)
    {
        if (enable)
        {
            Transform pos = doorPositions[Random.Range(0, doorPositions.Count)];
            Vector3 spawnPosition = new Vector3(pos.position.x, pos.position.y, 0); // Create new vector based on chosen position
            leverInstance = Instantiate(lever, spawnPosition, Quaternion.identity); // Instantiate the lever to spawn
            darkness = true;
            darknessCanvas.SetActive(true);
            Text leverText = darknessCanvas.transform.GetChild(1).GetComponent<Text>();
            int distance = (int) Vector2.Distance(p.GetComponent<Rigidbody2D>().position, GetLeverInstance().GetComponent<Rigidbody2D>().position);
            leverText.text = "Lever distance: " + distance;
        } else
        {
            darkness = false;
            darknessCanvas.SetActive(false);
        }
    }

    public GameObject GetLeverInstance()
    {
        return leverInstance;
    }

    public bool GetDarkness()
    {
        return darkness;
    }

    public IEnumerator WaveInformation(float time)
    {

        while (true)
        {
            if (startRoutine)
            {
                DisplayWaveInformation(false, true);
                startRoutine = false;
                currentMonsters = maxMonsters;
                mobToNextWaveText.text = "Mobs till next wave: " + currentMonsters;
                yield return new WaitForSeconds(time);
            }
            yield return new WaitForSeconds(respawnTimer);
            if (spawnedMonsters < maxMonsters)
            {
                SpawnEnemy();
            }
            else
            {
                break;
            }
        }
    }

    void DisplayWaveInformation(bool fade, bool textOnly)
    {
        if (!textOnly)
        {
            currentWave += 1;
            waveText.text = "WAVE " + (currentWave);
            currentWaveText.text = "Current Wave: " + currentWave;
        }
        if (fade)
        {
            waveText.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
            waveText.CrossFadeAlpha(1, 5.0f, false);
        } else
        {
            waveText.gameObject.GetComponent<CanvasRenderer>().SetAlpha(1f);
            waveText.CrossFadeAlpha(0, 5.0f, false);
        }
    }

    void SpawnEnemy()
    {
        setRangeByWave();
        int index = Random.Range(startRange, endRange);
        setDamageByEnemy(index);
        GameObject currentEnemy = enemies[index]; // Select a random enemy
        Transform pos = spawnPositions[Random.Range(0, spawnPositions.Count)]; // Select a random position
        //ShowRoomSpawn(pos.name);
        Vector3 spawnPosition = new Vector3(pos.position.x, pos.position.y, 0); // Create new vector based on chosen position
        GameObject spawn = Instantiate(currentEnemy, spawnPosition, Quaternion.identity); // Instantiate the monster to spawn
        GameObject canv = Instantiate(canvas); // Instantiate the canvas (hp bar)
        canv.transform.SetParent(spawn.transform); // Set the canvas as a child to the monster
        if (difficulty == "Expert")
        {
            spawn.GetComponent<AIPath>().maxSpeed += movementSpeed;
        }

        GameObject hog = GameObject.FindGameObjectWithTag("HedgeHog");

        if (hog == null)
        {
            spawn.GetComponent<AIDestinationSetter>().target = player; // Add the target (AI pathing) to player
        } else
        {
            spawn.GetComponent<AIDestinationSetter>().target = hog.transform;
        }
        spawn.GetComponent<Enemy>().SetPlayer(player);
        spawn.GetComponent<Enemy>().SetRespawner(this.gameObject);
        spawn.GetComponent<Enemy>().SetMaxDamage(maxDamage);
        spawn.GetComponent<Enemy>().SetMaxHealth(healthIncrease);
        spawn.GetComponent<Enemy>().SetMinCoin(minCoin);
        spawn.GetComponent<Enemy>().SetMaxCoin(maxCoin);
        spawn.GetComponent<Enemy>().SetCoin(coin);
        spawn.GetComponent<Enemy>().SetRedPot(redPot);
        spawn.GetComponent<Enemy>().SetBluePot(bluePot);
        spawn.GetComponent<Enemy>().SetInstantKill(instantKill);
        if (difficulty == "Hard" || difficulty == "Expert")
        {
            spawn.GetComponent<Enemy>().SetKnockback(0);
        } else
        {
            spawn.GetComponent<Enemy>().SetKnockback(knockback);
        }
        spawnedMonsters++;
    }

    void SpawnBoss()
    {
        waveText.text = "";
        GameObject currentEnemy = boss; // Select a random enemy
        Transform pos = doorPositions[Random.Range(0, doorPositions.Count)]; // Select a random position
        //ShowRoomSpawn(pos.name);
        bossPosition = pos;
        Vector3 spawnPosition = new Vector3(pos.position.x, pos.position.y, 0); // Create new vector based on chosen position
        GameObject spawn = Instantiate(currentEnemy, spawnPosition, Quaternion.identity); // Instantiate the monster to spawn
        spawn.GetComponent<Enemy>().SetMaxHealth(25000);
        spawn.GetComponent<Enemy>().SetCanvas(bossCanvas);
        spawn.GetComponent<Enemy>().SetPlayer(player);
        spawn.GetComponent<Enemy>().SetRespawner(this.gameObject);
        spawn.GetComponent<Enemy>().SetBoss(true);
        spawn.GetComponent<Enemy>().SetMinCoin(500);
        spawn.GetComponent<Enemy>().SetMaxCoin(750);
        spawn.GetComponent<Enemy>().SetCoin(coin);
        spawn.GetComponent<Enemy>().SetRedPot(redPot);
        spawn.GetComponent<Enemy>().SetBluePot(bluePot);
        spawn.GetComponent<Enemy>().SetSpectralSword(spectralSword);
        player.GetComponent<Player>().SetOxygenInterval_2(1, 1);
        if (difficulty == "Hard" || difficulty == "Expert")
        {
            spawn.GetComponent<Enemy>().SetKnockback(0);
        }
        else
        {
            spawn.GetComponent<Enemy>().SetKnockback(20);
        }
        bossSpawned = true;
    }

    public void SpawnMinion(GameObject bossSpawn)
    {
        setRangeByWave();
        int index = Random.Range(startRange, endRange);
        setDamageByEnemy(index);
        GameObject currentEnemy = enemies[index]; // Select a random enemy
        Transform pos = bossPosition; // Select a random position
        //ShowRoomSpawn(pos.name);
        Vector3 spawnPosition = new Vector3(pos.position.x, pos.position.y - 1.7f, 0); // Create new vector based on chosen position
        GameObject spawn = Instantiate(currentEnemy, spawnPosition, Quaternion.identity); // Instantiate the monster to spawn
        GameObject canv = Instantiate(canvas); // Instantiate the canvas (hp bar)
        canv.transform.SetParent(spawn.transform); // Set the canvas as a child to the monster
        Physics2D.IgnoreCollision(bossSpawn.GetComponent<CircleCollider2D>(), spawn.GetComponent<CircleCollider2D>());
        if (difficulty == "Expert")
        {
            spawn.GetComponent<AIPath>().maxSpeed += movementSpeed;
        }
        spawn.GetComponent<AIDestinationSetter>().target = player;
        spawn.GetComponent<Enemy>().SetPlayer(player);
        spawn.GetComponent<Enemy>().SetRespawner(this.gameObject);
        spawn.GetComponent<Enemy>().SetMaxDamage(maxDamage);
        spawn.GetComponent<Enemy>().SetMaxHealth(healthIncrease);
        spawn.GetComponent<Enemy>().SetMinCoin(minCoin);
        spawn.GetComponent<Enemy>().SetMaxCoin(maxCoin);
        spawn.GetComponent<Enemy>().SetCoin(coin);
        spawn.GetComponent<Enemy>().SetRedPot(redPot);
        spawn.GetComponent<Enemy>().SetBluePot(bluePot);
        if (difficulty == "Hard" || difficulty == "Expert")
        {
            spawn.GetComponent<Enemy>().SetKnockback(0);
        }
        else
        {
            spawn.GetComponent<Enemy>().SetKnockback(knockback);
        }
    }

    void SpawnHog()
    {
        Transform pos = spawnPositions[Random.Range(0, spawnPositions.Count)]; // Select a random position
        Vector3 spawnPosition = new Vector3(pos.position.x, pos.position.y, 0); // Create new vector based on chosen position
        Instantiate(hog, spawnPosition, Quaternion.identity); // Instantiate the monster to spawn
    }

    public void setDamageByEnemy(int index)
    {
        if (index == 0)
        {
            maxDamage = 50 + (currentWave == 1 ? 1 : currentWave * 5);
            healthIncrease = 500 + (currentWave == 1 ? 0 : currentWave * 10);
            knockback = 60;
            minCoin = 5;
            maxCoin = 20;
        } else if (index == 1)
        {
            maxDamage = 60 + (currentWave == 1 ? 1 : currentWave * 5);
            healthIncrease = 500 + (currentWave * 15);
            knockback = 45;
            minCoin = 25;
            maxCoin = 40;
        } else if (index == 2)
        {
            maxDamage = 70 + (currentWave == 1 ? 1 : currentWave * 5);
            healthIncrease = 500 + (currentWave * 25);
            knockback = 30;
            minCoin = 45;
            maxCoin = 60;
        } else if (index == 3)
        {
            maxDamage = 80 + (currentWave == 1 ? 1 : currentWave * 5);
            healthIncrease = 500 + (currentWave * 30);
            knockback = 15;
            minCoin = 65;
            maxCoin = 80;
        } else if (index == 4)
        {
            maxDamage = 90 + (currentWave == 1 ? 1 : currentWave * 5);
            healthIncrease = 500 + (currentWave * 40);
            knockback = 1;
            minCoin = 85;
            maxCoin = 100;
        }
    }

    public void setRangeByWave()
    {
        int specialMob = RoundDown(currentWave);
        //Debug.Log(specialMob);
        if (currentWave >= 0 && currentWave <= 5 || specialMob + 0 == currentWave)
        {
            startRange = 0;
        } else if (specialMob + 6 == currentWave) 
        {
            startRange = 1;
            endRange = 1;
        } else if (specialMob + 7 == currentWave)
        {
            startRange = 2;
            endRange = 2;
        } else if (specialMob + 8 == currentWave)
        {
            startRange = 3;
            endRange = 3;
        } else if (specialMob + 9 == currentWave)
        {
            startRange = 4;
            endRange = 4;
        } else
        {
            startRange = 0;
            if (currentWave >= 50)
            {
                endRange = 4;
            }
            else
            {
                endRange = (currentWave / 10) + 1;
            }
        }

    }

    private int RoundDown(int toRound)
    {
        return toRound - toRound % 10;
    }


    public void DecreaseCurrentMonsters()
    {
        if (bossSpawned) {
            return;
        }
        currentMonsters--;
        mobToNextWaveText.text = "Mobs till next wave: " + currentMonsters;
        if (currentMonsters <= 0)
        {
            if (respawnTimer >= 0.8f)
            {
                respawnTimer -= 0.1f;
            }        
            currentMonsters = 0;
            maxMonsters += 2;
            DisplayWaveInformation(true, false);
            StopCoroutine(coroutine);
            if (p.GetTaskSystem().GetWaveRequired() > -1)
            {
                if (currentWave > p.GetTaskSystem().GetWaveRequired())
                {
                    p.Die();
                    return;
                }
            }
            startRoutine = true;
            StartWaves();
        }
    }

    public void AddToDoors(int door)
    {
        Player p = player.GetComponent<Player>();
        if (p.GetTaskSystem().GetCurrentTask().Equals("Room"))
        {
            if (p.GetTaskSystem().GetRoomRequired() == door)
            {
                p.GetTaskSystem().CompleteTask(currentWave);
            }
        }
        doorsUnlocked.Add(door);
        if (doorsUnlocked.Count < 11)
        {
            p.StatusMessage("A weapons chest will now spawn in room " + (door == 2 ? "1 and 2" : "" + door) + ". \r\nYou've unlocked Fire " + p.getNameByGun(doorsUnlocked.Count) + ".", 5);
        } 
        UpdateSpawnPositions();
        UpdateDoorPositions();
    }

    public void QuickSpawn()
    {
        chestRespawner.GetComponent<ChestRespawner>().UpdateSpawnPositions(doorsUnlocked);
        chestRespawner.GetComponent<ChestRespawner>().SpawnAllChests();
    }

    public int DoorSize()
    {
        return doorsUnlocked.Count;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public string GetDifficulty()
    {
        return difficulty;
    }
}
