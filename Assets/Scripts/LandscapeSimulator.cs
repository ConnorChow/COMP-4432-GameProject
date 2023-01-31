using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using Unity.Collections;
using System;

public struct WFCTile {
    Tile tile;
    int[] sockets;
    public WFCTile SetParams(Tile t, int[] s)
    {
        this.tile = t;
        this.sockets = s;
        return this;
    }
    public Tile GetTile()
    {
        return tile;
    }
    public int[] GetSockets()
    {
        return sockets;
    }
    public void SetSockets(int direction, int SocketType)
    {
        this.sockets[direction] = SocketType;
    }
}

public struct Navigation {
    int Traversability;
}

public class LandscapeSimulator : MonoBehaviour {
    //Socketing info
    static int Empty = -1;
    static int Grass = 0;
    static int Dirt = 1;
    static int GrassVDirt = 2;
    static int DirtVGrass = 3;

    //directional info
    static int left = 0;
    static int right = 2;
    static int up = 1;
    static int down = 3;

    //Navigational Info
    static int passable = 0;
    static int avoid = 1;
    static int obstacle = 2;

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
    public int TerrainSize = 16;

    WFCTile[] Map2D;
    public Tilemap GroundTileMap;

    private void CollapseTerrain(int posx, int posy) {
        if (posx < TerrainSize && posy < TerrainSize) {
            int[] TileOptions = new int[18];
            int count = 0;
            //get tiles around inst
            if (posx > 0)
            {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    left,
                    Map2D[(posx - 1) * TerrainSize + posy].GetSockets()[right]
                );
            }
            if (posx < TerrainSize - 1)
            {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    right,
                    Map2D[(posx + 1) * TerrainSize + posy].GetSockets()[left]
                );
            }
            if (posy < TerrainSize - 1)
            {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    up,
                    Map2D[posx * TerrainSize + posy + 1].GetSockets()[down]
                );
            }
            if (posy > 0)
            {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    down,
                    Map2D[posx * TerrainSize + posy - 1].GetSockets()[up]
                );
            }
            //find tiletype matching socket
            for (int type = 0; type < tiles.Length; type++) {
                if (Map2D[posx * TerrainSize + posy].GetSockets()[left] == tiles[type].GetSockets()[left] || Map2D[posx * TerrainSize + posy].GetSockets()[left] == Empty)
                { }
                else { continue; }
                if (Map2D[posx * TerrainSize + posy].GetSockets()[right] == tiles[type].GetSockets()[right] || Map2D[posx * TerrainSize + posy].GetSockets()[right] == Empty)
                { }
                else { continue; }
                if (Map2D[posx * TerrainSize + posy].GetSockets()[up] == tiles[type].GetSockets()[up] || Map2D[posx * TerrainSize + posy].GetSockets()[up] == Empty)
                { }
                else { continue; }
                if (Map2D[posx * TerrainSize + posy].GetSockets()[down] == tiles[type].GetSockets()[down] || Map2D[posx * TerrainSize + posy].GetSockets()[down] == Empty)
                { }
                else { continue; }

                if (type <= 1 && UnityEngine.Random.Range(0, 10) < 1)
                    continue;

                TileOptions[count] = type;
                count++;
            }
            //set tile and socket
            int RandomFittingTile = UnityEngine.Random.Range(0, count - 1);
            Map2D[posx * TerrainSize + posy] = tiles[TileOptions[RandomFittingTile]];

            GroundTileMap.SetTile(new Vector3Int(posx - (TerrainSize / 2), posy - (TerrainSize / 2), 0), Map2D[posx * TerrainSize + posy].GetTile());
        }

    }

    // Start is called before the first frame update
    void Start() {
        //[0] = left, [1] = up, [2] = right, [3] = down
        //GrassVDirt = Grass on down/left, Dirt on up/right
        tiles[0].SetParams(DirtFull, new int[4] { Dirt, Dirt, Dirt, Dirt });
        tiles[1].SetParams(GrassFull, new int[4] { Grass, Grass, Grass, Grass });
        tiles[2].SetParams(GrassDirtDown, new int[4] { DirtVGrass, Grass, DirtVGrass, Dirt });
        tiles[3].SetParams(GrassDirtDownLeft, new int[4] { DirtVGrass, Grass, Grass, DirtVGrass });
        tiles[4].SetParams(GrassDirtLeft, new int[4] { Dirt, DirtVGrass, Grass, DirtVGrass });
        tiles[5].SetParams(GrassDirtUpLeft, new int[4] { GrassVDirt, DirtVGrass, Grass, Grass });
        tiles[6].SetParams(GrassDirtUp, new int[4] { GrassVDirt, Dirt, GrassVDirt, Grass });
        tiles[7].SetParams(GrassDirtUpRight, new int[4] { Grass, GrassVDirt, GrassVDirt, Grass });
        tiles[8].SetParams(GrassDirtRight, new int[4] { Grass, GrassVDirt, Dirt, GrassVDirt });
        tiles[9].SetParams(GrassDirtDownRight, new int[4] { Grass, Grass, DirtVGrass, GrassVDirt });
        tiles[10].SetParams(DirtGrassDown, new int[4] { GrassVDirt, Dirt, GrassVDirt, Grass });
        tiles[11].SetParams(DirtGrassDownLeft, new int[4] { GrassVDirt, Dirt, Dirt, GrassVDirt });
        tiles[12].SetParams(DirtGrassLeft, new int[4] { Grass, GrassVDirt, Dirt, GrassVDirt });
        tiles[13].SetParams(DirtGrassUpLeft, new int[4] { DirtVGrass, GrassVDirt, Dirt, Dirt });
        tiles[14].SetParams(DirtGrassUp, new int[4] { DirtVGrass, Grass, DirtVGrass, Dirt });
        tiles[15].SetParams(DirtGrassUpRight, new int[4] { Dirt, DirtVGrass, DirtVGrass, Dirt });
        tiles[16].SetParams(DirtGrassRight, new int[4] { Dirt, DirtVGrass, Grass, DirtVGrass });
        tiles[17].SetParams(DirtGrassDownRight, new int[4] { Dirt, Dirt, GrassVDirt, DirtVGrass });

        Map2D = new WFCTile[TerrainSize * TerrainSize];

        for (int x = 0; x < TerrainSize; x++) {
            for (int y = 0; y < TerrainSize; y++) {
                Map2D[x * TerrainSize + y].SetParams(null, new int[4] { Empty, Empty, Empty, Empty });
            }
        }
        for (int x = 0; x < TerrainSize; x++) {
            for (int y = 0; y < TerrainSize; y++) {
                CollapseTerrain(x, y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
