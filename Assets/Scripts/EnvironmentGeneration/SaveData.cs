using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveData : MonoBehaviour {
    [SerializeField] private LandscapeSaveData lsd;

    public void SaveEnvironment(LandscapeSimulator landscapeSim, FoliageSimulator foliageSim) {
        lsd = new LandscapeSaveData(landscapeSim);
        string EnvData = JsonUtility.ToJson(lsd);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/LandscapeData.json", EnvData);
    }

    public LandscapeSaveData LoadEnvironment() {
        lsd = JsonUtility.FromJson<LandscapeSaveData>(System.IO.File.ReadAllText(Application.persistentDataPath + "/LandscapeData.json"));
        return lsd;
    }
}
