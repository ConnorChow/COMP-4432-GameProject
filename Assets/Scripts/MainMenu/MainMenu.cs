using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Linq;
using System;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private myNetworkManager mnetworkManager;
    //[SerializeField] private GameObject mainMenuPage = null;
    //[SerializeField] private GameObject joinPage = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button hostSlot1 = null;
    [SerializeField] private Button hostSlot2 = null;
    [SerializeField] private Button hostSlot3 = null;
    [SerializeField] private Button joinButton = null;

    AudioSource audioSrc;


    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        joinButton.onClick.AddListener(JoinLobby);
        hostSlot1.onClick.AddListener(HostLobby);
        hostSlot2.onClick.AddListener(HostLobby);
        hostSlot3.onClick.AddListener(HostLobby);

    }

    public void HostLobby() {
        audioSrc.Play();
        Debug.Log("Hosting...");
        PlayerPrefs.SetInt("hosting", 1);
        networkManager.StartHost();
    }

    public void JoinLobby() {
        audioSrc.Play();
        if (ipAddressInputField.text == "") { return; };
        PlayerPrefs.SetInt("hosting", 0);
        //joinButton.interactable = false;
        string ipAddress = ipAddressInputField.text;
        Debug.Log(ipAddress);
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }

    

}
