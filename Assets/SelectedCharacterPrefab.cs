using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelectedCharacterPrefab : MonoBehaviour
{
    public GameObject[] characters;

    void Start()
    {
        int selectedCharacterIndex = CharacterSelectionData.LoadSelectedCharacter();
        characters[selectedCharacterIndex].SetActive(true);
    }

    void Update()
    {
        // code to control the player character
    }
}
