using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button hostSlot1 = null;
    [SerializeField] private Button hostSlot2 = null;
    [SerializeField] private Button hostSlot3 = null;
    [SerializeField] private Button joinButton = null;
    [SerializeField] private Button retryButton = null;
    [SerializeField] private Button quitButton = null;

    [SerializeField] bool deletePrefs;

    private void Awake()
    {
        if (deletePrefs)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    private void Start()
    {
        try
        {
            joinButton.onClick.AddListener(JoinLobby);
            hostSlot1.onClick.AddListener(HostLobby);
            hostSlot2.onClick.AddListener(HostLobby);
            hostSlot3.onClick.AddListener(HostLobby);
            retryButton.onClick.AddListener(Retry);
            quitButton.onClick.AddListener(Quit);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public void HostLobby() {
        Debug.Log("Hosting...");
        PlayerPrefs.SetInt("hosting", 1);
        networkManager.StartHost();
    }

    public void JoinLobby()
    {
        if (ipAddressInputField.text == "") { return; };
        PlayerPrefs.SetInt("hosting", 0);
        //joinButton.interactable = false;
        string ipAddress = ipAddressInputField.text;
        Debug.Log(ipAddress);
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }

    public void Retry()
    {
        HostMode.ActivateHostScene();
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }

}
