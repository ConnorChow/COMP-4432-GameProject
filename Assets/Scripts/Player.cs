using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public float PlayerSpeed = 2.5f;

    public Rigidbody2D rb;

    //public Camera playerCam;


    void HandleMovement()
    {
        // check if not local player
        if (!isLocalPlayer) { return; }

        // handle player movement
        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveY = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(MoveX, MoveY) * PlayerSpeed;

        //playerCam.transform.position = new Vector3(rb.position.x, rb.position.y, -10);
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(GetAssetBundle());
        rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator GetAssetBundle() {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("http://www.my-server.com");
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.Send();
 
        if(www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            AssetBundle bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
        }
    }

    // Update is called once per frame
    void Update() {
        HandleMovement();
    }
}
