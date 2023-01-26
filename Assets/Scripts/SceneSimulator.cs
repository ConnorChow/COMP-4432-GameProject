using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct WFCTile
{
    Tile tile;
    int[] sockets;
    public WFCTile setParams(Tile t, int[] s)
    {
        this.tile = t;
        this.sockets = s;
        return this;
    }
}

public class SceneSimulator : MonoBehaviour {
    //Socketing info
    static int Grass = 0;
    static int Dirt = 1;
    static int GrassVDirt = 2;
    static int DirtVGrass = 3;

    WFCTile[] tiles = new WFCTile[18];

    [Header("Ground TileTypes")]
    public Tile DirtFull;
    public Tile GrassFull;

    public Tile GrassDirtDown;
    public Tile GrassDirtDownLeft;
    public Tile GrassDirtLeft;
    public Tile GrassDirtUpLeft;
    public Tile GrassDirtUp;
    public Tile GrassDirtUpRight;
    public Tile GrassDirtRight;
    public Tile GrassDirtDownRight;

    public Tile DirtGrassDown;
    public Tile DirtGrassDownLeft;
    public Tile DirtGrassLeft;
    public Tile DirtGrassUpLeft;
    public Tile DirtGrassUp;
    public Tile DirtGrassUpRight;
    public Tile DirtGrassRight;
    public Tile DirtGrassDownRight;

    [Header("Tile Maps")]
    public int TerrainSize = 512;
    public Tilemap GroundTiles;

    private void CollapseTerrain(int posx, int posy)
    {

    }
    private void InitTerrain()
    {
        //[0] = left, [1] = up, [2] = right, [3] = down
        //GrassVDirt = Grass on down/left, Dirt on up/right
        tiles[0].setParams(DirtFull, new int[4] { Dirt, Dirt, Dirt, Dirt });
        tiles[1].setParams(GrassFull, new int[4] { Grass, Grass, Grass, Grass });
        tiles[2].setParams(GrassDirtDown, new int[4] { DirtVGrass, Grass, DirtVGrass, Dirt });
        tiles[3].setParams(GrassDirtDownLeft, new int[4] { DirtVGrass, Grass, Grass, DirtVGrass });
        tiles[4].setParams(GrassDirtLeft, new int[4] { Dirt, DirtVGrass, Grass, DirtVGrass });
        tiles[5].setParams(GrassDirtUpLeft, new int[4] { GrassVDirt, DirtVGrass, Grass, Grass });
        tiles[6].setParams(GrassDirtUp, new int[4] { GrassVDirt, Dirt, GrassVDirt, Grass });
        tiles[7].setParams(GrassDirtUpRight, new int[4] { Grass, GrassVDirt, GrassVDirt, Grass });
        tiles[8].setParams(GrassDirtRight, new int[4] { Grass, GrassVDirt, Dirt, GrassVDirt });
        tiles[9].setParams(GrassDirtDownRight, new int[4] { Grass, Grass, DirtVGrass, GrassVDirt });
        tiles[10].setParams(DirtGrassDown, new int[4] { GrassVDirt, Dirt, GrassVDirt, Grass });
        tiles[11].setParams(DirtGrassDownLeft, new int[4] { GrassVDirt, Dirt, Dirt, GrassVDirt });
        tiles[12].setParams(DirtGrassLeft, new int[4] { Grass, GrassVDirt, Dirt, GrassVDirt });
        tiles[13].setParams(DirtGrassUpLeft, new int[4] { DirtVGrass, DirtVGrass, Dirt, Dirt });
        tiles[14].setParams(DirtGrassUp, new int[4] { DirtVGrass, Grass, DirtVGrass, Dirt });
        tiles[15].setParams(DirtGrassUpRight, new int[4] { Dirt, DirtVGrass, DirtVGrass, Dirt });
        tiles[16].setParams(DirtGrassRight, new int[4] { Dirt, DirtVGrass, Grass, DirtVGrass });
        tiles[17].setParams(DirtGrassDownRight, new int[4] { Dirt, Dirt, GrassVDirt, DirtVGrass });

        GroundTiles.ResizeBounds();

        int x = Random.Range(0,TerrainSize);
        int y = Random.Range(0, TerrainSize);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
