using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class myNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        Debug.Log("Server Started");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    public override void OnClientConnect()
    {
        Debug.Log("Connected to Server");
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("Disconnected from Server");
    }
}
