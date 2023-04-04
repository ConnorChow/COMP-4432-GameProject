using Mirror;
using OPS.AntiCheat.Field;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct BurnQueueClassifier {
    public int[] BurnQueue;
    public int[] ttl;
    public float[] health;
}

public struct BushesClassifier {
    public int[] bushLocX;
    public int[] bushLocY;
}
public struct RocksClassifier {
    public int[] rockLocX;
    public int[] rockLocY;
}

public class EnvironmentSync : NetworkBehaviour {
    [SerializeField] LandscapeSimulator landscape;
    [SerializeField] FoliageSimulator foliage;
    int chunkSize = 4096; //FUCKING MAKE INTO 2^x
    int foliageChunkSize = 1024;
    public int chunkQty = 0;
    private Map2dClassifier[] m2d;

    void Start () {
    }

    public override void OnStartClient() {
        base.OnStartClient();
        if (!PlayerPrefs.HasKey("hosting") || (PlayerPrefs.HasKey("hosting") && PlayerPrefs.GetInt("hosting") != 1)) {
            if (landscape.loadInFire) {
                landscape.initializeTileTypes();
                foliage.InitializeData();
                RequestSynchronizeClientTerrain();
            }
        }
    }
    void Update() {
        
    }

    [Command(requiresAuthority = false)]
    public void RequestSynchronizeClientTerrain() {

        // for loading foliage data
        FoliageSaveData fsd = new FoliageSaveData(foliage);

        // init bush sync data
        chunkQty = Mathf.CeilToInt((float)fsd.BerryTilesX.Length / (float)foliageChunkSize);
        int numInstances = fsd.BerryTilesX.Length;
        BushesClassifier bc = new BushesClassifier {
            bushLocX = fsd.BerryTilesX,
            bushLocY = fsd.BerryTilesY
        };
        SynchronizeBushes(bc);

        //init rock sync data
        chunkQty = Mathf.CeilToInt((float)fsd.RockTilesX.Length / (float)foliageChunkSize);
        numInstances = fsd.RockTilesX.Length;
        RocksClassifier rc = new RocksClassifier {
            rockLocX = fsd.RockTilesX,
            rockLocY = fsd.RockTilesY
        };
        SynchronizeRocks(rc);

        //// Load bushes instances
        //for (int chunkInterval = 0; chunkInterval < chunkQty; chunkInterval++) {
        //    if (remainingInstances > chunkSize) {
        //        remainingInstances -= chunkSize;
        //        BushesClassifier bc = new BushesClassifier {
        //            length = chunkSize,
        //            bushLocX = new int[chunkSize],
        //            bushLocY = new int[chunkSize]
        //        };
        //        for (int i = 0; i < chunkSize; i++) {
        //            int j = (chunkInterval * chunkQty) + i;
        //            Debug.Log("i = " + j);
        //            bc.bushLocX[i] = fsd.BerryTilesX[j];
        //            bc.bushLocY[i] = fsd.BerryTilesY[j];
        //        }
        //        SynchronizeBushes(bc);
        //    } else {
        //        BushesClassifier bc = new BushesClassifier {
        //            length = remainingInstances,
        //            bushLocX = new int[remainingInstances],
        //            bushLocY = new int[remainingInstances]
        //        };
        //        for (int i = 0; i < remainingInstances; i++) {
        //            int j = (chunkInterval * chunkQty) + i;
        //            Debug.Log("i = " + j);
        //            bc.bushLocX[i] = fsd.BerryTilesX[j];
        //            bc.bushLocY[i] = fsd.BerryTilesY[j];
        //        }
        //        SynchronizeBushes(bc);
        //    }
        //}



        //// Load rocks instances
        //for (int rockChunkInterval = 0; rockChunkInterval < chunkQty; rockChunkInterval++) {
        //    if (remainingInstances > chunkSize) {
        //        remainingInstances -= chunkSize;
        //        RocksClassifier rc = new RocksClassifier {
        //            length = chunkSize,
        //            rockLocX = new int[chunkSize],
        //            rockLocY = new int[chunkSize]
        //        };
        //        for (int i = 0; i < chunkSize; i++) {
        //            int j = (rockChunkInterval * chunkQty) + i;
        //            Debug.Log("i = " + j);
        //            rc.rockLocX[i] = fsd.RockTilesX[j];
        //            rc.rockLocY[i] = fsd.RockTilesY[j];
        //        }
        //        SyncronizeRocks(rc);
        //    } else {
        //        RocksClassifier rc = new RocksClassifier {
        //            length = remainingInstances,
        //            rockLocX = new int[remainingInstances],
        //            rockLocY = new int[remainingInstances]
        //        };
        //        for (int i = 0; i < remainingInstances; i++) {
        //            int j = (rockChunkInterval * chunkQty) + i;
        //            Debug.Log("i = " + j);
        //            rc.rockLocX[i] = fsd.RockTilesX[j];
        //            rc.rockLocY[i] = fsd.RockTilesY[j];
        //        }
        //        SyncronizeRocks(rc);
        //    }
        //}

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
        BurnQueueClassifier bq = new BurnQueueClassifier {
            BurnQueue = new int[landscape.burnQueue.Count],
            ttl = new int[landscape.burnQueue.Count],
            health = new float[landscape.burnQueue.Count]
        };
        for (int i = 0; i < landscape.burnQueue.Count; i++) {
            bq.BurnQueue[i] = landscape.BurnQueue.ElementAt(i);
            bq.ttl[i] = landscape.BurnData[bq.BurnQueue[i]].TimeToLive;
            bq.health[i] = landscape.BurnData[bq.BurnQueue[i]].Health;
        }
        SynchronizeBurnQueue(bq);
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

        //landscape.isMapLoaded = isLast;
    }

    [ClientRpc]
    public void SynchronizeBurnQueue(BurnQueueClassifier bq) {
        //if (landscape.loadInFire == false || isServer) return;
        int index;
        for (int i = 0; i < bq.BurnQueue.Length; i++) {
            index = bq.BurnQueue[i];
            landscape.BurnCell(index, bq.ttl[i]);
            landscape.BurnData[index].Health = bq.health[i];
        }
        landscape.isMapLoaded = true;
    }

    [ClientRpc]
    public void SynchronizeBushes(BushesClassifier bc) {
        if (isServer) return;
        foliage.LoadBushFromClassifier(bc);
    }
    [ClientRpc]
    public void SynchronizeRocks(RocksClassifier bc) {
        if (isServer) return;
        foliage.LoadRocksFromClassifier(bc);
    }
}