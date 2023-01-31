using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Harvestable
{
    public int resourceType;
    public int resourceCount;
    public Vector2Int[] usedTiles;
    public Tile[] tile;
}

public class FoliageSimulator : MonoBehaviour {
    int rock = 0;
    int berries = 1;
    int branches = 2;

    [Header("Picked Bushes")]
    public Tile BerryBushPicked1;
    public Tile BerryBushPicked2;
    public Tile BerryBushPicked3;
    public Tile BerryBushPicked4;

    public Tile BigBerryBushPickedUpperLeft;
    public Tile BigBerryBushPickedUpperRight;
    public Tile BigBerryBushPickedLowerRight;
    public Tile BigBerryBushPickedLowerLeft;

    [Header("Berry Bushes")]
    public Tile BerryBush1;
    public Tile BerryBush2;
    public Tile BerryBush3;
    public Tile BerryBush4;

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
    
    [Header("Big Rock")]
    public Tile BigRockLeft;
    public Tile BigRockRight;

    [Header("Foliage Tilemap")]
    public static int TerrainSize;
    public int BushesQty;
    public int RocksQty;
    public int clumpSize;
    public int foliageSpacing;
    public Tilemap FoliageTiles;

    [Header("Referenced Scripts")]
    public LandscapeSimulator TerrainScript;

    private void GenerateClump(int x, int y, int ttl, int index) {
        if (ttl > 0) {

        }
    }

    // Start is called before the first frame update
    void Start() {
        Harvestable[] harvestables = new Harvestable[(BushesQty + RocksQty) * clumpSize];
        for (int i = 0; i < BushesQty; i++) {
            int x = Random.Range(0, TerrainSize);
            int y = Random.Range(0, TerrainSize);

            GenerateClump(x, y, clumpSize, i * clumpSize);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
