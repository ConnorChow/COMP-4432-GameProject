using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEngine.UI;

public class CheatingDetector : MonoBehaviour
{
    public string playerDataPath = "Assets/Resources/updated_player_data.csv"; // Path to your updated_player_data.csv

    public GameObject player; // Reference to the player game object
    public Text gameStatusText; // Reference to the UI Text element to show game status

    void Start()
    {
        // Get the player's name from the Player script
        string playerName = player.GetComponent<Player>().playerName;

        // Read the CSV file
        string[] lines = File.ReadAllLines(playerDataPath);

        // Parse the CSV and store the data in a Dictionary
        Dictionary<string, bool> playerCheatingStatus = new Dictionary<string, bool>();
        bool header = true;
        foreach (string line in lines)
        {
            if (header) // Skip the header line
            {
                header = false;
                continue;
            }

            string[] columns = line.Split(',');
            string name = columns[0]; // Get the player name from the first column
            bool isCheating = Convert.ToBoolean(columns[columns.Length - 1]); // Get the 'cheating_detected' column value
            playerCheatingStatus.Add(name, isCheating);
        }

        // Check if the player is marked as cheating and take action
        if (playerCheatingStatus.ContainsKey(playerName) && playerCheatingStatus[playerName])
        {
            Debug.Log("Player is cheating. Access to the game is denied.");
            // Prevent the player from playing the game

        }
        else
        {
            Debug.Log("Player is not cheating. Access to the game is granted.");
            // Allow the player to play the game

        }
    }
}
