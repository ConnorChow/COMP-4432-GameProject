using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class mNetworkManager : NetworkManager
{


    public override void OnStartServer()
    {
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
