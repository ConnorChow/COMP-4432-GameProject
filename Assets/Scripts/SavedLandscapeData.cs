using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SavedLandscapeData : ScriptableObject {
    public Tilemap GroundData;
    public Tilemap FoliageData;

    public WFCTile[] Map2D;
    public BurnComponent[] BurnData;
}
