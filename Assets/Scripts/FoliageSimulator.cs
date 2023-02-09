using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

using OPS.AntiCheat;
using OPS.AntiCheat.Field;

public struct BushBerriesComponent : ECS_Component {
    public ProtectedInt32 BerryCount;
    public ProtectedInt32 Countdown;
    public Vector2Int Tile;
}
public class BushEntityManagement : ECS_EntityComponentManagement {
    private FoliageSimulator FoliageSystem;
    public BushEntityManagement(int MaxEntities, FoliageSimulator CFS) : base(MaxEntities) {
        FoliageSystem = CFS;
    }
    public void AddEntity(BushBerriesComponent NewBerriesData) {
        base.AddEntity();
        if (ActiveEntities <= MaxEntities) {
            int ComponentIndex = indexQueue[entityQueue[ActiveEntities - 1]];
            FoliageSystem.BushBerriesData[ComponentIndex] = NewBerriesData;
        }
    }
    new public void RemoveEntity(int entity) {
        int Passable = 0;
        if (ActiveEntities > 0) {
            int RemovedIndex = indexQueue[entity];
            FoliageSystem.FoliageTilemap.SetTile(new Vector3Int(FoliageSystem.BushBerriesData[RemovedIndex].Tile.x, FoliageSystem.BushBerriesData[RemovedIndex].Tile.y, 0), null);
            int index = FoliageSystem.BushBerriesData[RemovedIndex].Tile.x * FoliageSystem.LandScapeSimulator.TerrainSize + FoliageSystem.BushBerriesData[RemovedIndex].Tile.y;
            FoliageSystem.LandScapeSimulator.NavComponent[index].Traversability = Passable;

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
                    break;
                case berries:
                    FoliageTilemap.SetTile(new Vector3Int(x, y, 0), FreshBerryBushes[Random.Range(0, 4)]);
                    LandScapeSimulator.NavComponent[IndexX * LandScapeSimulator.TerrainSize + IndexY].Traversability = avoid;
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

    // Start is called before the first frame update
    void Start() {
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
    }

    // Update is called once per frame
    void Update() {

    }
}
