using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Harvestable
{
    public int resourceType;
    public int resourceCount;
    public int x;
    public int y;
}

public class FoliageSimulator : MonoBehaviour {
    int rock = 0;
    int berries = 1;

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
    public int BushesQty;
    public int RocksQty;
    public int clumpSize;
    public int foliageSpacing;
    public Tilemap FoliageTilemap;

    Harvestable[] harvestables;

    [Header("Referenced Scripts")]
    public LandscapeSimulator LandScapeSimulator;

    private void GenerateClump(int x, int y, int TimeToLive, int StartIndex, int type) {
        if (TimeToLive > 0) {
            int CurrentIndex = StartIndex + TimeToLive - 1;
            harvestables[CurrentIndex] = new Harvestable
            {
                resourceType= type,
                resourceCount= Random.Range(2,5),
                x= x,
                y= y
            };
            switch (type)
            {
                case 0:
                    FoliageTilemap.SetTile(new Vector3Int);
                    break;
            }
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
        int ClumpCount = (int)Mathf.Pow(Mathf.Floor(Mathf.Sqrt(BushesQty + RocksQty)), 2);
        int BoundsXY = (int)Mathf.Sqrt(ClumpCount);
        harvestables = new Harvestable[ClumpCount * clumpSize];
        int flipFlop = 0;
        int ClumpSpacing = BoundsXY/LandScapeSimulator.TerrainSize;
        for (int x = 0; x < Mathf.Sqrt(ClumpCount); x++) {
            for (int y = 0; y < Mathf.Sqrt(ClumpCount); y++) {
                GenerateClump(x * ClumpSpacing, y * ClumpSpacing, clumpSize, (x * BoundsXY + y) * ClumpCount, flipFlop);
                if (flipFlop > 0) flipFlop = 0;
                else flipFlop = 1;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
