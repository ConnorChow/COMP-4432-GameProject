using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HostilesMarkerBehaviour : NetworkBehaviour {
    public GameObject marker;
    public int terrainSize;
    public int mapDimensions = 150;
    // Start is called before the first frame update
    void Start() {
        terrainSize = GameObject.Find("Landscape").GetComponent<LandscapeSimulator>().TerrainSize;
        rtrans = GetComponent<RectTransform>();
    }

    RectTransform rtrans;
    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) return;
        if (marker != null) {
            float posx = mapDimensions * (marker.transform.position.x / terrainSize + 0.5f);
            float posy = mapDimensions * (marker.transform.position.x / terrainSize + 0.5f);
            rtrans.transform.position = new Vector2 (posx, posy);
        } else {
            Destroy(gameObject);
        }
    }
}
