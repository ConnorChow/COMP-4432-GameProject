using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveData : MonoBehaviour {
    [SerializeField] private LandscapeSaveData lsd;

    public void SaveEnvironment(LandscapeSimulator landscapeSim, FoliageSimulator foliageSim) {
        lsd = new LandscapeSaveData(landscapeSim, foliageSim);
        string EnvData = JsonUtility.ToJson(lsd);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/LandscapeData.json", EnvData);
    }

    public LandscapeSaveData LoadEnvironment() {
        lsd = JsonUtility.FromJson<LandscapeSaveData>(Application.persistentDataPath + "/LandscapeData.json");
        return lsd;
    }
}

[System.Serializable]
public class LandscapeSaveData {
    public Tilemap GroundData;
    public Tilemap FoliageData;

    public int TerrainSize;

    //Landscape Components
    public BurnComponent[] BurnData;

    //Foliage Components
    public BushBerriesComponent[] BushBerryData;
    public Dictionary<BushTilingComponent, int> BushTilingData;

    public LandscapeSaveData(LandscapeSimulator ls, FoliageSimulator fs) {
        this.GroundData = ls.GroundTileMap;
        this.BurnData = ls.BurnData;

        this.FoliageData = fs.FoliageTilemap;
        this.BushBerryData = fs.BushBerriesData;
        this.BushTilingData = fs.BushTilingData;
    }
}
