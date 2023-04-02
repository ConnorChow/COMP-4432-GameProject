using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentSync : NetworkBehaviour {
    [SerializeField] LandscapeSimulator landscape;
    [SerializeField] FoliageSimulator foliage;
    int chunkSize = 4096; //FUCKING MAKE INTO 2^x
    public int chunkQty = 0;
    private Map2dClassifier[] m2d;

    void Start () {
    }

    public override void OnStartClient() {
        base.OnStartClient();
        if (!PlayerPrefs.HasKey("hosting") || (PlayerPrefs.HasKey("hosting") && PlayerPrefs.GetInt("hosting") != 1)) {
            landscape.initializeTileTypes();
            RequestSynchronizeClientTerrain();
            /*for (int x = 0; x < landscape.TerrainSize; x++) {
                for (int y = 0; y < landscape.TerrainSize; y++) {
                    landscape.LoadTileFromLSD(x, y);
                }
            }*/
        }
    }
    void Update() {
        
    }

    [Command(requiresAuthority = false)]
    public void RequestSynchronizeClientTerrain() {
        if (!isServer) return;

        LandscapeSaveData lsd = new LandscapeSaveData(landscape);

        chunkQty = landscape.Map2D.Length / chunkSize;
        m2d = new Map2dClassifier[chunkQty];

        for (int chunkInterval = 0; chunkInterval < chunkQty; chunkInterval++) {
            m2d[chunkInterval] = new Map2dClassifier {
                chunkSize = chunkSize,
                chunkInterval = chunkInterval,
                map2DIndex = new int[chunkSize]
            };
            for (int i = 0; i < chunkSize; i++) {
                m2d[chunkInterval].map2DIndex[i] = lsd.map2DIndex[(chunkInterval * chunkSize) + i];
            }
            bool isLast = false;
            if (chunkInterval == chunkQty - 1) {
                isLast = true;
            }
            SynchronizeClientTerrain(m2d[chunkInterval], isLast);
        }


        FoliageSaveData fsd = new FoliageSaveData(foliage);
    }

    public int GetY(int index) {
        return index % landscape.TerrainSize;
    }
    public int GetX(int index) {
        return (index - GetY(index)) / landscape.TerrainSize;
    }

    [ClientRpc]
    public void SynchronizeClientTerrain(Map2dClassifier m2d, bool isLast) {
        Debug.Log("Load Chunk " + m2d.chunkInterval);
        int chunkSize = m2d.chunkSize;
        int chunkInterval = m2d.chunkInterval;
        int offset = chunkSize * chunkInterval;

        for (int i = 0; i < chunkSize; i++) {
            landscape.Map2D[i + offset] = landscape.tiles[
                m2d.map2DIndex[i]
            ];

            if (m2d.map2DIndex[i] == 0) landscape.NeutralizeTile(i + offset);
            else landscape.FlammefyTile(i + offset);
        }
        landscape.isMapLoaded = isLast;
    }
}
