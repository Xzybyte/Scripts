using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tasks
{
    private List<string> tasks = new List<string>();
    private List<string> taskDescriptions = new List<string>();
    private List<string> completedTasks = new List<string>();
    private string currentTask = "";
    private int taskIndex;
    private int roomRequired = -1;
    private int waveRequired;
    private int nextTaskWave;
    private GameObject taskInfo;
    private Player player;
    private string mode;
    private int completed;

    public Tasks(string mode, GameObject taskInfo, Player player)
    {
        this.taskInfo = taskInfo;
        this.player = player;
        int room;
        int byWave;
        this.mode = mode;
        if (mode.Equals("Normal"))
        {
            room = Random.Range(4, 7);
            byWave = 10;
            roomRequired = room;
            waveRequired = byWave;
            tasks = new List<string> { "Room", "Protect", "Light", "Arrow", "Fire", "Poison", "Kill" };
            taskDescriptions = new List<string> { "Open the doors to room " + room + " by Wave " + byWave + ". \r\nFailure to complete this task will result in death", "Find and protect the hedgehog for 3 waves. \r\nFailure to complete this task will result in death",
            "Find and activate the lever to turn the lights back on", "Kill atleast 5 monsters by using the Arrow Traps (Between room 3 and 5)", "Kill atleast 10 monsters by using Fire Traps (Between room 7 and 8)",
            "Kill atleast 15 monsters using Poison Traps (Between room 8 and 9)", "Find and kill the Spectral. Your Oxygen will drain faster the longer you keep it alive"};
        } else if (mode.Equals("Hard"))
        {
            room = Random.Range(7, 10);
            byWave = 9;
            roomRequired = room;
            waveRequired = byWave;
            tasks = new List<string> { "Room", "Protect", "Light", "Arrow", "Fire", "Poison", "Kill" };
            taskDescriptions = new List<string> { "Open the doors to room " + room + " by Wave " + byWave + ". \r\nFailure to complete this task will result in death", "Find and protect the hedgehog for 5 waves. \r\nFailure to complete this task will result in death",
            "Find and activate the lever to turn the lights back on", "Kill atleast 10 monsters by using the Arrow Traps (Between room 3 and 4)", "Kill atleast 15 monsters by using Fire Traps (Between room 7 and 8)",
            "Kill atleast 20 monsters using Poison Traps (Between room 10 and 11)", "Find and kill the Spectral. Your Oxygen will drain faster the longer you keep it alive"};
        } else if (mode.Equals("Expert"))
        {
            room = Random.Range(9, 12);
            byWave = 8;
            roomRequired = room;
            waveRequired = byWave;
            tasks = new List<string> { "Room", "Protect", "Light", "Arrow", "Fire", "Poison", "Kill" };
            taskDescriptions = new List<string> { "Open the doors to room " + room + " by Wave " + byWave + ".\r\nFailure to complete this task will result in death", "Find and protect the hedgehog for 7 waves. \r\nFailure to complete this task will result in death",
            "Find and activate the lever to turn the lights back on", "Kill atleast 15 monsters by using the Arrow Traps (Between room 4 and 5)", "Kill atleast 20 monsters by using Fire Traps (Between room 7 and 8)",
            "Kill atleast 25 monsters using Poison Traps (Between room 9 and 10)", "Find and kill the Spectral. Your Oxygen will drain faster the longer you keep it alive"};
        }

        if (Player.testing)
        {
            taskDescriptions.RemoveRange(0, 6);
            tasks.RemoveRange(0, 6);
            waveRequired = -1;
            nextTaskWave = 46;
        }
    }

    public void AutoStartTask()
    {
        taskIndex = 0;
        currentTask = tasks[taskIndex];
        tasks.RemoveAt(taskIndex);
        taskInfo.GetComponent<Text>().text = "Task Received! Press T to view it.";
        taskInfo.GetComponent<Animator>().enabled = true;
    }

    public void RandomizeTask()
    {
        taskIndex = 0;
        currentTask = tasks[taskIndex];
        taskInfo.GetComponent<Text>().text = "Task Received! Press T to View it.";
        taskInfo.GetComponent<Animator>().enabled = true;
        if (currentTask.Equals("Protect"))
        {
            if (mode.Equals("Normal"))
            {
                waveRequired = nextTaskWave + 3;
            } else if (mode.Equals("Hard"))
            {
                waveRequired = nextTaskWave + 5;
            } else if (mode.Equals("Expert"))
            {
                waveRequired = nextTaskWave + 7;
            }
        }
    }

    public void CompleteTask(int currentWave)
    {
        taskInfo.GetComponent<Animator>().enabled = false;
        player.TaskMessage("You have successfully completed your task.", 8);
        completedTasks.Add(currentTask);
        tasks.Remove(currentTask);
        taskDescriptions.RemoveAt(taskIndex);
        waveRequired = -1;
        nextTaskWave = currentWave + 5;
        currentTask = "";
        completed++;
    }

    public bool HasTask()
    {
        return currentTask.Length != 0;
    }

    public string GetCurrentTask()
    {
        return currentTask;
    }

    public string GetTaskDescription()
    {
        return taskDescriptions[taskIndex];
    }

    public int GetTaskIndex()
    {
        return taskIndex;
    }

    public int GetRoomRequired()
    {
        return roomRequired;
    }

    public int GetWaveRequired()
    {
        return waveRequired;
    }

    public int GetNextTaskWave()
    {
        return nextTaskWave;
    }

    public int GetTaskCompleted()
    {
        return completed;
    }

}
