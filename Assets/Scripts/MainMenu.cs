using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Net;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private NetworkManager networkManager;
    //[SerializeField] private GameObject mainMenuPage = null;
    //[SerializeField] private GameObject joinPage = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button hostSlot1 = null;
    [SerializeField] private Button hostSlot2 = null;
    [SerializeField] private Button hostSlot3 = null;
    [SerializeField] private Button joinButton = null;

    private void Start()
    {
        joinButton.onClick.AddListener(JoinLobby);
        hostSlot1.onClick.AddListener(HostLobby);
        hostSlot2.onClick.AddListener(HostLobby);
        hostSlot3.onClick.AddListener(HostLobby);
    }

    public void HostLobby() {
        Debug.Log("Hosting...");
        networkManager.StartHost();
    }

    public void JoinLobby()
    {
        if (ipAddressInputField.text == "") { return; };

        //joinButton.interactable = false;
        string ipAddress = ipAddressInputField.text;
        Debug.Log(ipAddress);
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }

}
