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
    [SerializeField] private Button hostButton = null;
    [SerializeField] private Button joinButton = null;

    private void Start()
    {
        joinButton.onClick.AddListener(JoinLobby);
        hostButton.onClick.AddListener(HostLobby);
    }

    public void HostLobby() {
        Debug.Log("Hosting...");
        networkManager.StartHost();
    }

    public void JoinLobby()
    {
        if (ipAddressInputField.text == "") { return; };

        joinButton.interactable = false;
        string ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }

}
