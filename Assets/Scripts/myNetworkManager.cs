using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Net;

public class myNetworkManager : NetworkManager
{
    EnemySpawner enemySpawner;
    [SerializeField] public GameObject swarmerPrefab;
    [SerializeField] public GameObject bigSwarmerPrefab;
    [SerializeField] public GameObject[] spawnLocations;

    public override void Start()
    {
        base.Start();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("Server Started");
        Debug.Log($"IP: {GetLocalIPv4()}");
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

        //NetworkClient.localPlayer.transform.SetPositionAndRotation((0,0,0));
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        Debug.Log("Disconnected from Server");
    }

    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }

    public void startSpawnEnemy()
    {
        StartCoroutine(spawnEnemy(enemySpawner.swarmerInterval, enemySpawner.swarmerPrefab));
        StartCoroutine(spawnEnemy(enemySpawner.bigSwarmerInterval, enemySpawner.bigSwarmerPrefab));
    }

    public IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        System.Random r = new System.Random();
        int rInt = r.Next(0, spawnLocations.Length);

        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, spawnLocations[rInt].transform.position, spawnLocations[rInt].transform.rotation); //Enemy spawn
        NetworkServer.Spawn(newEnemy);
        StartCoroutine(spawnEnemy(interval, enemy));
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
