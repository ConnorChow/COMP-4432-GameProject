using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionData : MonoBehaviour
{
    private const string selectedCharacterKey = "index";

    public static void SaveSelectedCharacter(int index)
    {
        PlayerPrefs.SetInt(selectedCharacterKey, index);
        PlayerPrefs.Save();
    }

    public static int LoadSelectedCharacter()
    {
        return PlayerPrefs.GetInt(selectedCharacterKey, 0);
    }
}