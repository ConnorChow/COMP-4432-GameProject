using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject mainMenuPage = null;
    [SerializeField] private GameObject joinPage = null;
    [SerializeField] private InputField ipAddressInputField = null;
    [SerializeField] private Button hostButton = null;
    [SerializeField] private Button joinButton = null;


    public void HostLobby() {
        networkManager.StartHost();
        mainMenuPage.SetActive(false);
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
        joinButton.interactable = false;
    }

}
