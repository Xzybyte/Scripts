using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestRespawner : MonoBehaviour
{

    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject smallChest;
    [SerializeField] private Text chestText;
    private float timer = 60;
    private int room = 0;
    private List<Transform> spawnPositions = new List<Transform>();
    private GameObject spawnedChest;
    private bool weaponsChest;

    void Start()
    {
        PopulateSpawnPositions();
    }

    void Update()
    {
        if (chestText.text.Length != 0)
        {
            timer -= Time.deltaTime;
            if (timer >= 0)
            {
                int minutes = Mathf.FloorToInt(timer / 60F);
                int seconds = Mathf.FloorToInt(timer % 60F);
                chestText.text = "A " + (weaponsChest ? "weapons":"buff") + " chest has spawned in Room " + (room) + ".\r\nTime till disappearance: " + minutes.ToString("00") + ":" + seconds.ToString("00");
            }
        }
    }

    public void SpawnChest()
    {
        if (spawnedChest != null)
        {
            Destroy(spawnedChest);
        }

        int chosenChest = Random.Range(0, 10);
        if (chosenChest <= 6)
        {
            weaponsChest = true;
        }
        int index = Random.Range(0, spawnPositions.Count);
        room = System.Int32.Parse(spawnPositions[index].name.Substring(6));
        Transform pos = spawnPositions[index]; // Select a random position
        Vector3 spawnPosition = new Vector3(pos.position.x, pos.position.y, 0);
        spawnedChest = Instantiate(weaponsChest ? chest : smallChest, spawnPosition, Quaternion.identity);
        StartCoroutine(DestroyChest(spawnedChest));
    }

    public void SpawnAllChests()
    {
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            room = System.Int32.Parse(spawnPositions[i].name.Substring(6));
            Transform pos = spawnPositions[i]; // Select a random position
            Vector3 spawnPosition = new Vector3(pos.position.x, pos.position.y, 0);
            spawnedChest = Instantiate(chest, spawnPosition, Quaternion.identity);
        }
    }

    private IEnumerator DestroyChest(GameObject spawnedChest)
    {
        chestText.text = "A " + (weaponsChest ? "weapons":"buff") + " chest has spawned in Room " + (room) + ".\r\nTime till disappearance: 01:00";
        yield return new WaitForSeconds(60);
        if (spawnedChest != null)
        {
            ResetTimer();
            if (weaponsChest ? !spawnedChest.GetComponent<Chest>().StopDestroy() : !spawnedChest.GetComponent<SmallChest>().StopDestroy())
            {
                Destroy(spawnedChest);
            }
        }
    }

    public void ResetTimer()
    {
        timer = 60;
        chestText.text = "";
    }

    private void PopulateSpawnPositions()
    {
        spawnPositions.Add(transform.GetChild(0));
    }

    public void UpdateSpawnPositions(List<int> doorsUnlocked)
    {
        for (int i = 0; i < doorsUnlocked.Count; i++)
        {
            for (int g = 0; g < transform.childCount; g++)
            {
                if (!spawnPositions.Contains(transform.GetChild(g)))
                {
                    int chestNum = System.Int32.Parse(transform.GetChild(g).name.Substring(6));
                    if (chestNum == doorsUnlocked[i])
                    {
                        spawnPositions.Add(transform.GetChild(g));
                    }
                }
            }
        }
    }
}
