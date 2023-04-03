using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyTracker : NetworkBehaviour {
    [SerializeField]public readonly SyncHashSet<GameObject> players = new SyncHashSet<GameObject>();
    [SerializeField]public readonly SyncHashSet<GameObject> enemies = new SyncHashSet<GameObject>();
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
