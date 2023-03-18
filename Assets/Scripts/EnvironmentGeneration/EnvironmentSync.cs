using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentSync : NetworkBehaviour {

    [SerializeField][SyncVar] private GameObject sceneSimulator;
    [SerializeField][SyncVar] private GameObject ground;
    [SerializeField][SyncVar] private GameObject foliage;
    [SerializeField][SyncVar] private GameObject fire;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
