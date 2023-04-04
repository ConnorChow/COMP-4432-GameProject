using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using OPS.AntiCheat;
using OPS.AntiCheat.Field;

public class SlotSelectionTool : MonoBehaviour {
    //identify slot index for each save slot (1, 2, 3, ...)
    [SerializeField] private ProtectedInt32 slotIndex;
    //text object for displaying the given save slot
    [SerializeField] private TMP_Text text;
    //access the local mainMenu path
    public MainMenu mainMenu;
    //permanent secure storage of slot name for duration of session
    private ProtectedString slotName;

    // Start is called before the first frame update
    void Start() {
        //identifier for the key describing save slot... either ACTIVE or EMPTY
        string prefName = "SlotSaved" + slotIndex;

        //if/else statement gets the state of the save slot
        if (PlayerPrefs.HasKey(prefName)) {
            //we use this key to fill the display and inform our setup
        } else { //if there is no key for this save slot then create a new key for it
            PlayerPrefs.SetString(prefName, "EMPTY");
        }

        //stores the data we need securely and displays it to the player 
        slotName = PlayerPrefs.GetString(prefName);
        text.text = "SLOT " + slotIndex + " - " + slotName;
    }

    public void SelectSlot() {
        //identify slot index for each save slot (1, 2, 3, ...)
        string prefName = "SlotSaved" + slotIndex.ToString();

        //specify slot we must load for landscape simulator
        PlayerPrefs.SetInt("loadSlot", slotIndex);
        PlayerPrefs.SetString(prefName, "ACTIVE");

        //either generate a new map to fill a slot
        if (slotName == "EMPTY") {
            PlayerPrefs.SetInt("loadMap", 0);

        } else { //or, load a map from a slot
            PlayerPrefs.SetInt("loadMap", 0);
        }
    }
}
