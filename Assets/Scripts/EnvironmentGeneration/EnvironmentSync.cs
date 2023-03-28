using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentSync : NetworkBehaviour {
    [SerializeField] LandscapeSimulator landscape;
    [SerializeField] FoliageSimulator foliage;
    [SerializeField] int chunkSize = 2048; //FUCKING MAKE INTO 2^x
    private bool loaded = false;
    private int chunkInterval;
    private int chunkQty;
    private Map2dClassifier[] m2d;

    void Start () {
        chunkQty = landscape.Map2D.Length / chunkSize;
        m2d = new Map2dClassifier[chunkQty];
        loaded = true;
    }

    void Update() {
        if (!loaded && chunkInterval < chunkQty) {
            Debug.Log("General Syncing GameObjects");
            RequestSynchronizeClientTerrain();
            chunkInterval++;
            if (chunkInterval >= chunkQty) {
                loaded = true;
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void RequestSynchronizeClientTerrain() {
        LandscapeSaveData lsd = new LandscapeSaveData(landscape);

        landscape.initializeTileTypes();

        m2d[chunkInterval] = new Map2dClassifier {
            chunkSize = chunkSize,
            chunkInterval = chunkInterval,
            map2DIndex = new int[chunkSize],
            BurnState = new int[chunkSize],
            TimeToLive = new int[chunkSize],
            Health = new float[chunkSize]
        };
        for (int i = 0; i < chunkSize; i++) {
            m2d[chunkInterval].map2DIndex[i] = lsd.map2DIndex[(chunkInterval * chunkSize) + i];
            m2d[chunkInterval].BurnState[i] = lsd.BurnState[(chunkInterval * chunkSize) + i];
            m2d[chunkInterval].TimeToLive[i] = lsd.TimeToLive[(chunkInterval * chunkSize) + i];
            m2d[chunkInterval].Health[i] = lsd.Health[(chunkInterval * chunkSize) + i];
        }
        SynchronizeClientTerrain(m2d[chunkInterval]);
        SynchronizeTerrain(m2d[chunkInterval]);

        FoliageSaveData fsd = new FoliageSaveData(foliage);
    }

    [ClientRpc]
    public void SynchronizeClientTerrain(Map2dClassifier m2d) {
        SynchronizeTerrain(m2d);
    }
    private void SynchronizeTerrain(Map2dClassifier m2d) {
        if (!loaded) {
            landscape.LoadFromClassifier(m2d);
        }
    }
}
