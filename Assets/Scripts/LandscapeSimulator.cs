using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using Unity.Collections;
using System;
using Unity.Mathematics;
using static UnityEngine.RuleTile.TilingRuleOutput;

public struct WFCTile {
    Tile tile;
    int[] sockets;
    public WFCTile SetParams(Tile t, int[] s) {
        this.tile = t;
        this.sockets = s;
        return this;
    }
    public Tile GetTile() {
        return tile;
    }
    public int[] GetSockets() {
        return sockets;
    }
    public void SetSockets(int direction, int SocketType) {
        this.sockets[direction] = SocketType;
    }
}

public struct Navigation {
    public int Traversability;
}

public struct BurnComponent {
    public int BurnState;
    public float Health;
    public int TimeToLive;
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

    //Burn States
    static int Normal = 0;
    static int Burning = 1;
    static int Burned = 2;

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

    public WFCTile[] Map2D;
    public Navigation[] NavComponent;
    public BurnComponent[] BurnData;

    public Tilemap GroundTileMap;
    public Tilemap FireGrid;

    [Header("Simulation")]
    public float FireDamagePerSecond = 6.0f;
    public float NormalHealth = 50.0f;
    public float BurningHealth = 90.0f;
    public int FireLife = 10;
    public AnimatedTile FireSprite;
    public Tile BurnedTile;

    BurnComponent FlammableTile;
    BurnComponent SafeTile;

    public int[] BurnQueue;
    public int BurningEntities = 0;

    private void CollapseTerrain(int posx, int posy) {
        if (posx < TerrainSize && posy < TerrainSize) {
            int[] TileOptions = new int[22];
            int count = 0;
            //get tiles around inst
            if (posx > 0) {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    left,
                    Map2D[(posx - 1) * TerrainSize + posy].GetSockets()[right]
                );
            }
            if (posx < TerrainSize - 1) {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    right,
                    Map2D[(posx + 1) * TerrainSize + posy].GetSockets()[left]
                );
            }
            if (posy < TerrainSize - 1) {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    up,
                    Map2D[posx * TerrainSize + posy + 1].GetSockets()[down]
                );
            }
            if (posy > 0) {
                Map2D[posx * TerrainSize + posy].SetSockets(
                    down,
                    Map2D[posx * TerrainSize + posy - 1].GetSockets()[up]
                );
            }
            //find tiletype matching socket
            for (int type = 0; type < tiles.Length; type++) {
                if (Map2D[posx * TerrainSize + posy].GetSockets()[left] == tiles[type].GetSockets()[left] || Map2D[posx * TerrainSize + posy].GetSockets()[left] == Empty) { } else { continue; }
                if (Map2D[posx * TerrainSize + posy].GetSockets()[right] == tiles[type].GetSockets()[right] || Map2D[posx * TerrainSize + posy].GetSockets()[right] == Empty) { } else { continue; }
                if (Map2D[posx * TerrainSize + posy].GetSockets()[up] == tiles[type].GetSockets()[up] || Map2D[posx * TerrainSize + posy].GetSockets()[up] == Empty) { } else { continue; }
                if (Map2D[posx * TerrainSize + posy].GetSockets()[down] == tiles[type].GetSockets()[down] || Map2D[posx * TerrainSize + posy].GetSockets()[down] == Empty) { } else { continue; }

                if (type == 0) {
                    TileOptions[count] = type;
                    count++;
                    TileOptions[count] = type;
                    count++;
                }

                TileOptions[count] = type;
                count++;
            }
            //set tile and socket
            int RandomFittingIndex = UnityEngine.Random.Range(0, count);
            int SelectedIndex = TileOptions[RandomFittingIndex];
            Map2D[posx * TerrainSize + posy] = tiles[SelectedIndex];

            if (SelectedIndex == 0) {
                BurnData[posx * TerrainSize + posy] = SafeTile;
            } else {
                BurnData[posx * TerrainSize + posy] = FlammableTile;
            }

            GroundTileMap.SetTile(new Vector3Int(posx - (TerrainSize / 2), posy - (TerrainSize / 2), 0), Map2D[posx * TerrainSize + posy].GetTile());
        }

    }

    private int GetY(int index) {
        return index % TerrainSize;
    }
    private int GetX(int index) {
        return (index - GetY(index)) / TerrainSize;
    }

    private int GetIndex(int x, int y) {
        return x * TerrainSize + y;
    }
    private void BurnCell(int CurrentIndex, int ttl) {
        if (BurnData[CurrentIndex].BurnState == Normal) {

            BurnData[CurrentIndex] = new BurnComponent {
                BurnState = Burning,
                Health = BurningHealth,
                TimeToLive = ttl
            };
            FireGrid.SetTile(new Vector3Int(
                GetX(CurrentIndex) - (TerrainSize / 2),
                GetY(CurrentIndex) - (TerrainSize / 2), 0),
                FireSprite);
            BurnQueue[BurningEntities] = CurrentIndex;
            BurningEntities += 1;
        }
    }
    private void FinishBurnCell(int CurrentIndex, int QueueIndex) {
        if (BurnData[CurrentIndex].BurnState == Burning) {
            
            BurningEntities -= 1;
            BurnQueue[QueueIndex] = BurnQueue[BurningEntities];

            BurnData[CurrentIndex] = new BurnComponent {
                BurnState = Burned,
                Health = 0,
                TimeToLive = 0
            };

            FireGrid.SetTile(new Vector3Int(
                GetX(CurrentIndex) - (TerrainSize / 2),
                GetY(CurrentIndex) - (TerrainSize / 2), 0),
                null);
            GroundTileMap.SetTile(new Vector3Int(
                GetX(CurrentIndex) - (TerrainSize / 2),
                GetY(CurrentIndex) - (TerrainSize / 2), 0),
                BurnedTile);
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

        FlammableTile = new BurnComponent {
            BurnState = Normal,
            Health = NormalHealth,
            TimeToLive = 0
        };
        SafeTile = new BurnComponent {
            BurnState = Burned,
            Health = 0,
            TimeToLive = 0
        };

        Map2D = new WFCTile[TerrainSize * TerrainSize];
        NavComponent = new Navigation[TerrainSize * TerrainSize];
        BurnData = new BurnComponent[TerrainSize * TerrainSize];
        BurnQueue = new int[TerrainSize * TerrainSize];

        for (int x = 0; x < TerrainSize; x++) {
            for (int y = 0; y < TerrainSize; y++) {
                Map2D[x * TerrainSize + y].SetParams(null, new int[4] { Empty, Empty, Empty, Empty });
                NavComponent[x * TerrainSize + y].Traversability = passable;
            }
        }
        for (int x = 0; x < TerrainSize; x++) {
            for (int y = 0; y < TerrainSize; y++) {
                CollapseTerrain(x, y);
            }
        }

        BurnCell(GetIndex(TerrainSize / 2, TerrainSize / 2), FireLife);
    }

    // Update is called once per frame
    void Update() {
        float Elapsed = Time.deltaTime;
        int index;
        int LeftNeighbor;
        int RightNeighbor;
        int UpNeighbor;
        int DownNeighbor;

        int[] CellsToRemove = new int[TerrainSize];
        int[] IndexToRemove = new int[TerrainSize];
        int PullCount = 0;
        //Debug.Log("Queue Size: " + BurningEntities);
        for (int i = 0; i < BurningEntities; i++) {
            index = BurnQueue[i];
            BurnData[index].Health -= FireDamagePerSecond * Elapsed;
            Debug.Log(BurnData[index].BurnState);
            if (BurnData[index].Health <= 0.0f) {
                CellsToRemove[PullCount] = index;
                IndexToRemove[PullCount] = i;
                PullCount++;
                continue;
            } else if (BurnData[index].TimeToLive > 0) {
                LeftNeighbor = GetIndex(GetX(index) - 1, GetY(index));
                if (BurnData[LeftNeighbor].BurnState == Normal && GetX(index) > 0) {
                    BurnData[LeftNeighbor].Health -= FireDamagePerSecond * Elapsed;
                    if (BurnData[LeftNeighbor].Health < 0) {
                        BurnCell(LeftNeighbor, BurnData[index].TimeToLive - 1);
                    }
                }
                RightNeighbor = GetIndex(GetX(index) + 1, GetY(index));
                if (BurnData[RightNeighbor].BurnState == Normal && GetX(index) < TerrainSize - 1) {
                    BurnData[RightNeighbor].Health -= FireDamagePerSecond * Elapsed;
                    if (BurnData[RightNeighbor].Health < 0) {
                        BurnCell(RightNeighbor, BurnData[index].TimeToLive - 1);
                    }
                }
                UpNeighbor = GetIndex(GetX(index), GetY(index) + 1);
                if (BurnData[UpNeighbor].BurnState == Normal && GetY(index) < TerrainSize - 1) {
                    BurnData[UpNeighbor].Health -= FireDamagePerSecond * Elapsed;
                    if (BurnData[UpNeighbor].Health < 0) {
                        BurnCell(UpNeighbor, BurnData[index].TimeToLive - 1);
                    }
                }
                DownNeighbor = GetIndex(GetX(index), GetY(index) - 1);
                if (BurnData[DownNeighbor].BurnState == Normal && GetY(index) > 0) {
                    BurnData[DownNeighbor].Health -= FireDamagePerSecond * Elapsed;
                    if (BurnData[DownNeighbor].Health < 0) {
                        BurnCell(DownNeighbor, BurnData[index].TimeToLive - 1);
                    }
                }
            }
        }
        for (int i = 0; i < PullCount; i++) {
            FinishBurnCell(CellsToRemove[i], IndexToRemove[i]);
        }
    }
}
