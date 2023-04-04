using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HostilesMarkerBehaviour : NetworkBehaviour {
    public GameObject marker;
    public float terrainSize;
    public float mapDimensions = 150;
    // Start is called before the first frame update
    void Start() {
        terrainSize = GameObject.Find("Landscape").GetComponent<LandscapeSimulator>().TerrainSize;
        rtrans = GetComponent<RectTransform>();
    }

    RectTransform rtrans;
    // Update is called once per frame
    void Update() {
        //if (!isLocalPlayer) return;
        if (marker != null) {
            float posx = mapDimensions * (marker.transform.position.x / terrainSize + 0.5f);
            float posy = mapDimensions * (marker.transform.position.y / terrainSize - 0.5f);
            rtrans.transform.localPosition = new Vector3 (posx, posy, 0);
            //transform.localPosition = new Vector3(posx, posy, 0);
            //Debug.Log(new Vector3(posx, posy, 0));
        } else {
            Destroy(gameObject);
        }
    }
}
