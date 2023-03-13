using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

using Mirror;
using UnityEngine.Networking;

using OPS.AntiCheat;
using OPS.AntiCheat.Field;
using System.Linq;
using System.IO;

public struct BushBerriesComponent : ECS_Component {
    public ProtectedInt32 BerryCount;
    public ProtectedInt32 Countdown;
    public Vector2Int Tile;
}

public struct BushTilingComponent : ECS_Component {
    public Vector2Int Tile;
}

public class BushEntityManagement : ECS_EntityComponentManagement {
    private FoliageSimulator FoliageSystem;
    public BushEntityManagement(int MaxEntities, FoliageSimulator CFS) : base(MaxEntities) {
        FoliageSystem = CFS;
    }
    public void AddEntity(BushBerriesComponent NewBerriesData) {
        AddEntity();
        if (ActiveEntities <= MaxEntities) {
            int ComponentIndex = indexQueue[entityQueue[ActiveEntities - 1]];
            FoliageSystem.BushBerriesData[ComponentIndex] = NewBerriesData;
            FoliageSystem.BushTilingData.Add(new BushTilingComponent { Tile = NewBerriesData.Tile }, ComponentIndex);
        }
    }
    new public void RemoveEntity(int entity) {
        int Passable = 0;
        if (ActiveEntities > 0) {
            int RemovedIndex = indexQueue[entity];

            FoliageSystem.FoliageTilemap.SetTile(
                new Vector3Int(FoliageSystem.BushBerriesData[RemovedIndex].Tile.x - (FoliageSystem.LandScapeSimulator.TerrainSize / 2),
                FoliageSystem.BushBerriesData[RemovedIndex].Tile.y - (FoliageSystem.LandScapeSimulator.TerrainSize/2),
                0), null);

            int index = FoliageSystem.BushBerriesData[RemovedIndex].Tile.x * FoliageSystem.LandScapeSimulator.TerrainSize + FoliageSystem.BushBerriesData[RemovedIndex].Tile.y;
            FoliageSystem.LandScapeSimulator.NavComponent[index].Traversability = Passable;

            FoliageSystem.BushTilingData.Remove(new BushTilingComponent {
                Tile = FoliageSystem.BushBerriesData[RemovedIndex].Tile
            }); ;

            FoliageSystem.BushBerriesData[RemovedIndex] = FoliageSystem.BushBerriesData[ActiveEntities - 1];
        }
        base.RemoveEntity(entity);
    }
}

public class FoliageSimulator : MonoBehaviour {
    const int rock = 0;
    const int berries = 1;

    //Navigational Info
    static ProtectedInt32 passable = 0;
    static ProtectedInt32 avoid = 1;
    static ProtectedInt32 obstacle = 2;

    [Header("Picked Bushes")]
    public Tile BerryBushPicked1;
    public Tile BerryBushPicked2;
    public Tile BerryBushPicked3;
    public Tile BerryBushPicked4;
    Tile[] PickedBerryBushes;

    public Tile BigBerryBushPickedUpperLeft;
    public Tile BigBerryBushPickedUpperRight;
    public Tile BigBerryBushPickedLowerRight;
    public Tile BigBerryBushPickedLowerLeft;

    [Header("Berry Bushes")]
    public Tile BerryBush1;
    public Tile BerryBush2;
    public Tile BerryBush3;
    public Tile BerryBush4;
    Tile[] FreshBerryBushes;

    public Tile BigBerryBushUpperLeft;
    public Tile BigBerryBushUpperRight;
    public Tile BigBerryBushLowerRight;
    public Tile BigBerryBushLowerLeft;

    [Header("Rocks")]
    public Tile Rock1;
    public Tile Rock2;
    public Tile Rock3;
    public Tile Rock4;
    public Tile Rock5;
    public Tile Rock6;
    public Tile Rock7;
    public Tile Rock8;
    public Tile Rock9;
    Tile[] Rocks;

    [Header("Big Rock")]
    public Tile BigRockLeft;
    public Tile BigRockRight;

    [Header("Foliage Tilemap")]
    //public static int TerrainSize;
    public ProtectedInt32 ClumpQty;
    public ProtectedInt32 ClumpSize;
    public ProtectedInt32 foliageSpacing;
    public Tilemap FoliageTilemap;

    public ProtectedInt32 MaxBushInstances = 2048;
    public BushBerriesComponent[] BushBerriesData;
    public Dictionary<BushTilingComponent, int> BushTilingData = new Dictionary<BushTilingComponent, int>();
    public HashSet<Vector2Int> RockTilingData = new HashSet<Vector2Int>();
    public BushEntityManagement FoliageData;

    [Header("Referenced Scripts")]
    public LandscapeSimulator LandScapeSimulator;

    private void GenerateClump(int x, int y, int TimeToLive, int type) {
        int IndexX = x + LandScapeSimulator.TerrainSize / 2;
        int IndexY = y + LandScapeSimulator.TerrainSize / 2;

        if (TimeToLive > 0 && LandScapeSimulator.NavComponent[IndexX * LandScapeSimulator.TerrainSize + IndexY].Traversability == passable) {

            switch (type) {
                case rock:
                    FoliageTilemap.SetTile(new Vector3Int(x, y, 0), Rocks[Random.Range(0, 9)]);
                    LandScapeSimulator.NavComponent[IndexX * LandScapeSimulator.TerrainSize + IndexY].Traversability = obstacle;
                    LandScapeSimulator.NeutralizeTile(IndexX * LandScapeSimulator.TerrainSize + IndexY);
                    //Add new Rock Location for save data
                    RockTilingData.Add(new Vector2Int(IndexX, IndexY));
                    break;
                case berries:
                    FoliageTilemap.SetTile(new Vector3Int(x, y, 0), FreshBerryBushes[Random.Range(0, 4)]);
                    LandScapeSimulator.NavComponent[IndexX * LandScapeSimulator.TerrainSize + IndexY].Traversability = avoid;
                    LandScapeSimulator.FlammefyTile(IndexX * LandScapeSimulator.TerrainSize + IndexY);
                    BushBerriesComponent newBerryData = new BushBerriesComponent {
                        BerryCount = Random.Range(2, 5),
                        Countdown = 0,
                        Tile = new Vector2Int(IndexX, IndexY)
                    };
                    FoliageData.AddEntity(newBerryData);
                    break;
            }
            int NextX;
            int NextY;
            if (Random.Range(0, 2) > 0) {
                NextX = x + Random.Range(1, foliageSpacing);
            } else {
                NextX = x - Random.Range(1, foliageSpacing);
            }
            if (Random.Range(0, 2) > 0) {
                NextY = y + Random.Range(1, foliageSpacing);
            } else {
                NextY = y - Random.Range(1, foliageSpacing);
            }
            if (x < -LandScapeSimulator.TerrainSize / 2 + foliageSpacing)
                NextX = x + Random.Range(1, foliageSpacing);
            if (x > LandScapeSimulator.TerrainSize / 2 - foliageSpacing)
                NextY = x - Random.Range(1, foliageSpacing);
            if (y < -LandScapeSimulator.TerrainSize / 2 + foliageSpacing)
                NextY = y + Random.Range(1, foliageSpacing);
            if (y > LandScapeSimulator.TerrainSize / 2 - foliageSpacing)
                NextY = y - Random.Range(1, foliageSpacing);

            GenerateClump(NextX, NextY, TimeToLive - 1, type);
        } else if (LandScapeSimulator.NavComponent[IndexX * LandScapeSimulator.TerrainSize + IndexY].Traversability != passable) {
            int NextX = x;
            int NextY = y;
            if (Random.Range(0, 2) > 0) {
                NextX = x + 1;
            } else {
                NextX = x - 1;
            }
            if (Random.Range(0, 2) > 0) {
                NextY = y + 1;
            } else {
                NextY = y - 1;
            }
            if (x < -LandScapeSimulator.TerrainSize / 2 + 2)
                NextX = x + 1;
            if (x > LandScapeSimulator.TerrainSize / 2 - 2)
                NextX = x - 1;
            if (y < -LandScapeSimulator.TerrainSize / 2 + 2)
                NextY = y + 1;
            if (y > LandScapeSimulator.TerrainSize / 2 - 2)
                NextY = y - 1;
            GenerateClump(NextX, NextY, TimeToLive, type);
        }
    }

    private ProtectedBool tryLoad;
    public ProtectedInt32 saveSlot = -1;

    // Start is called before the first frame update
    void Start() {

        //Load Playerprefs for generating or loading Landscape
        if (PlayerPrefs.HasKey("loadMap")) {
            //Try to determine if landscape needs loading
            switch (PlayerPrefs.GetInt("loadMap")) {
                //0 represents false
                case 0: tryLoad = false; break;
                //1 represents true
                case 1: tryLoad = true; break;
            }
            //Try to load from a slot or generate a new one
            if (PlayerPrefs.HasKey("loadSlot") && tryLoad) {
                saveSlot = PlayerPrefs.GetInt("loadSlot");
            } else {
                //by default override slot 0 if there is no incoming data
                PlayerPrefs.SetInt("numSlots", 1);
                PlayerPrefs.SetInt("loadSlot", 1);
                //ensure map does not try loading as there definitely does not exist said slot
                tryLoad = false;
            }
        } else {
            PlayerPrefs.SetInt("loadMap", 0);
            tryLoad = false;
        }


        PickedBerryBushes = new Tile[4] {
            BerryBushPicked1,
            BerryBushPicked2,
            BerryBushPicked3,
            BerryBushPicked4
        };
        FreshBerryBushes = new Tile[4] {
            BerryBush1,
            BerryBush2,
            BerryBush3,
            BerryBush4
        };
        Rocks = new Tile[9]
        {
            Rock1,
            Rock2,
            Rock3,
            Rock4,
            Rock5,
            Rock6,
            Rock7,
            Rock8,
            Rock9
        };

        FoliageData = new BushEntityManagement(MaxBushInstances, this);
        BushBerriesData = new BushBerriesComponent[MaxBushInstances];

        tryLoad = LandScapeSimulator.tryLoadMap;
        //If we don't want to load and/or a load file does not exist, generate new foliage
        if (tryLoad && LoadData(saveSlot)) {
            Debug.Log("Loading Saved Foliage");
        } else {
            Debug.Log("Generating new Foliage");
            if (ClumpQty * ClumpSize > MaxBushInstances) {
                ClumpQty = (MaxBushInstances - (MaxBushInstances % ClumpSize)) / ClumpSize;
            }
            ClumpQty = (int)Mathf.Floor(Mathf.Sqrt(ClumpQty));
            ProtectedInt32 AxisBounds = ClumpQty;
            ClumpQty *= ClumpQty;
            ProtectedInt32 ClumpSpacing = LandScapeSimulator.TerrainSize / AxisBounds;
            ProtectedInt32 flipFlop = 0;
            for (int x = 0; x < AxisBounds; x++) {
                for (int y = 0; y < AxisBounds; y++) {
                    GenerateClump(
                        (x * ClumpSpacing) - (LandScapeSimulator.TerrainSize / 2),
                        (y * ClumpSpacing) - (LandScapeSimulator.TerrainSize / 2),
                        ClumpSize, flipFlop);
                    if (Random.Range(0, 2) > 0) {
                        if (flipFlop != rock) flipFlop = rock;
                        else flipFlop = berries;
                    }
                }
            }
            SaveData(saveSlot);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void SaveData(int slot) {
        Debug.Log("Saving");

        FoliageSaveData fsd = new FoliageSaveData(this);

        string data = JsonUtility.ToJson(fsd);

        File.WriteAllText(Application.persistentDataPath + "/FoliageData" + slot + ".json", data);
    }

    public bool LoadData(int slot) {
        if (File.Exists(Application.persistentDataPath + "/FoliageData" + slot + ".json")) {
            FoliageSaveData fsd = new FoliageSaveData();
            string data = File.ReadAllText(Application.persistentDataPath + "/FoliageData" + slot + ".json");
            fsd = JsonUtility.FromJson<FoliageSaveData>(data);

            //fill all data values
            for (int i = 0; i < fsd.BerryCount.Length; i++) {
                Vector2Int tile = new Vector2Int(fsd.BerryTilesX[i], fsd.BerryTilesY[i]);

                FoliageTilemap.SetTile(new Vector3Int(tile.x - LandScapeSimulator.TerrainSize / 2, tile.y - LandScapeSimulator.TerrainSize / 2, 0), FreshBerryBushes[Random.Range(0, 4)]);
                LandScapeSimulator.NavComponent[tile.x * LandScapeSimulator.TerrainSize + tile.y].Traversability = avoid;
                LandScapeSimulator.FlammefyTile(tile.x * LandScapeSimulator.TerrainSize + tile.y);

                FoliageData.AddEntity(new BushBerriesComponent {
                    BerryCount = fsd.BerryCount[i],
                    Countdown = fsd.BerryCount[i],
                    Tile = tile
                });
                //Debug.Log("Load Bush " + i);
            }

            for (int i = 0; i < fsd.RockTilesX.Length; i++) {
                Vector2Int tile = new Vector2Int(fsd.RockTilesX[i], fsd.RockTilesY[i]);

                FoliageTilemap.SetTile(new Vector3Int(tile.x - LandScapeSimulator.TerrainSize / 2, tile.y - LandScapeSimulator.TerrainSize / 2, 0), Rocks[Random.Range(0, 9)]);
                
                LandScapeSimulator.NavComponent[tile.x * LandScapeSimulator.TerrainSize + tile.y].Traversability = obstacle;
                LandScapeSimulator.NeutralizeTile(tile.x * LandScapeSimulator.TerrainSize + tile.y);
                RockTilingData.Add(new Vector2Int(tile.x, tile.y));
                //Debug.Log("Load Rock " + i);
            }
            return true;
        } else {
            return false;
        }
    }
}

[System.Serializable]
public class FoliageSaveData {
    public int[] BerryCount;
    public int[] Countdown;
    public int[] BerryTilesX;
    public int[] BerryTilesY;

    public int[] RockTilesX;
    public int[] RockTilesY;

    public FoliageSaveData() { }
    public FoliageSaveData(FoliageSimulator fs) {
        BushBerriesComponent[] bushes = fs.BushBerriesData;

        int bushArrayLength = fs.BushTilingData.Count;

        BerryCount= new int[bushArrayLength];
        Countdown= new int[bushArrayLength];
        BerryTilesX = new int[bushArrayLength];
        BerryTilesY = new int[bushArrayLength];

        for (int i = 0; i < bushArrayLength; i++) {
            BerryCount[i] = bushes[i].BerryCount;
            Countdown[i] = bushes[i].Countdown;
            BerryTilesX[i] = bushes[i].Tile.x;
            BerryTilesY[i] = bushes[i].Tile.y;
        }

        HashSet<Vector2Int> rockEntities = fs.RockTilingData;
        RockTilesX = new int[rockEntities.Count];
        RockTilesY = new int[rockEntities.Count];

        for (int i = 0; i < RockTilesX.Length; i++) {
            RockTilesX[i] = rockEntities.ElementAt(i).x;
            RockTilesY[i] = rockEntities.ElementAt(i).y;
        }
    }
}