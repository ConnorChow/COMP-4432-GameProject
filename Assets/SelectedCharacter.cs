using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelectedCharacter : MonoBehaviour
{
    public GameObject[] characters;

    public void SelectCharacter(int index)
    {
        CharacterSelectionData.SaveSelectedCharacter(index);
    }
}
