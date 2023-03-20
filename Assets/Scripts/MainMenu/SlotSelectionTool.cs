using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class SlotSelectionTool : MonoBehaviour {
    [SerializeField] private int slotIndex;
    [SerializeField] private TMP_Text text;
    public MainMenu mainMenu;
    private bool isEmpty;
    private string slotName;
    // Start is called before the first frame update
    void Start() {
        string prefName = "SlotSaved" + slotIndex;
        if (PlayerPrefs.HasKey(prefName)) {
            if (PlayerPrefs.GetString(prefName) == "EMPTY") {
                isEmpty = false;
            } else {
                isEmpty = true;
            }
        } else {
            PlayerPrefs.SetString(prefName, "EMPTY");
            isEmpty = false;
        }
        slotName = PlayerPrefs.GetString(prefName);
        text.text = "SLOT " + slotIndex + " - " + slotName;
    }

    public void SelectSlot() {
        string prefName = "SlotSaved" + slotIndex;
        Debug.Log("Launching from slot: " + slotIndex);
        if (slotName == "EMPTY") {
            PlayerPrefs.SetInt("loadMap", 0);

        } else {
            PlayerPrefs.SetInt("loadMap", 0);
        }
        PlayerPrefs.SetInt("loadSlot", slotIndex);
        PlayerPrefs.SetString(prefName, "ACTIVE");
    }
}
