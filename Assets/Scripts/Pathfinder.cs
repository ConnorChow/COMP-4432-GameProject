using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OPS.AntiCheat;
using OPS.AntiCheat.Field;

public struct NavComponent : ECS_Component {
    public ProtectedVector2Int CurrentTile;
    public ProtectedVector2Int NextTile;
    public ProtectedVector2Int Destination;
    public ProtectedBool MustTravel;
}

public struct GameComponent : ECS_Component {
    public GameObject GameObject;
}

public struct TileNeighbors : ECS_Component {
    public int Left;
    public int Right;
    public int Up;
    public int Down;
}

public struct LocalNavToolComponent : ECS_Component {
    public Dictionary<int, TileNeighbors> Neighbors;
    public int[] Distance;
}

public class Pathfinder_EntityComponentManagement : ECS_EntityComponentManagement {
    private Pathfinder pathfinder;
    public Pathfinder_EntityComponentManagement(int MaxEntities, Pathfinder pathfinder) : base(MaxEntities) {
        this.pathfinder = pathfinder;
        pathfinder.NavigationData = new NavComponent[this.MaxEntities];
        pathfinder.LocalNavigationData = new LocalNavToolComponent[this.MaxEntities];
        
        int radius = pathfinder.TileRadius;
        int tileArea = (radius * 2)^2;

        for (int i = 0; i < this.MaxEntities; i++) {
            pathfinder.LocalNavigationData[i].Distance = new int[tileArea];
        }
    }

    public void AddEntity(ProtectedVector2Int EntityPosition) {
        AddEntity();
    }

    new public void RemoveEntity(int entity) {
        base.RemoveEntity(entity);
    }
}

public class Pathfinder : MonoBehaviour {
    public ProtectedInt32 TileRadius = 32;
    public NavComponent[] NavigationData;
    public LocalNavToolComponent[] LocalNavigationData;
    public Pathfinder() {

    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
