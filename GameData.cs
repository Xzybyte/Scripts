using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    private bool defeatedGame;

    public void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Menu"))
        {
            FindObjectOfType<AudioManager>().Stop("theme");
            FindObjectOfType<AudioManager>().Play("menutheme");
        } else
        {
            FindObjectOfType<AudioManager>().Stop("menutheme");
            FindObjectOfType<AudioManager>().Play("theme");
        }
    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/GameData.dat");
        SaveData data = new SaveData
        {
            defeatedGame = true
        };
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/GameData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/GameData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            defeatedGame = data.defeatedGame;
        }
    }

    public bool HasDefeatedGame()
    {
        return defeatedGame;
    }
}

[Serializable]
class SaveData
{
    public bool defeatedGame;
}
