using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Net;

public class mNetworkManager : NetworkManager
{
    public override void Start()
    {
        base.Start();

        Debug.Log($"IP: {IPAddress.Broadcast}");
    }

    public override void OnStartHost()
    {
        //ServerChangeScene("Main");
        base.OnStartHost();
        Debug.Log("Server Started");
    }

    public override void OnStartServer()
    {
        //ServerChangeScene("Main");
        base.OnStartServer();

        Debug.Log("Server Started");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.Log("Connected to Server");
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        Debug.Log("Disconnected from Server");
    }


    public void SpawnEnemy(GameObject enemy)
    {
        NetworkServer.Spawn(enemy);
    }

    //public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    base.OnServerAddPlayer(conn);

    //    if (SceneManager.GetActiveScene().name == menuScene)
    //    {
    //        NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
    //        NetworkServerAddPlayerForConnection(conn, roomPlayerInstance - gameObject);
    //    }
    //}

}
