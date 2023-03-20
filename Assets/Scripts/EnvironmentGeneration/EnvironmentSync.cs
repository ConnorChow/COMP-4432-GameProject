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
}
