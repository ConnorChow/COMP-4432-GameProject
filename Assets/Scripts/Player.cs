using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    // Sync Variable from Server to Client
    [SyncVar(hook = nameof(helloChange))]
    public int helloCount = 0;

    [SyncVar] public int playerHealth = 10;

    public float PlayerSpeed = 2.5f;

    public Rigidbody2D rb;

    public Camera playerCam;


    void HandleMovement()
    {
        // check if not local player
        if (!isLocalPlayer) { return; }

        // handle player movement
        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveY = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(MoveX, MoveY) * PlayerSpeed;

        playerCam.transform.position = new Vector3(rb.position.x, rb.position.y, -10);
    }

    // Start is called before the first frame update
    void Start() {
        //StartCoroutine(GetAssetBundle());
        rb = GetComponent<Rigidbody2D>();
    }

    //IEnumerator GetAssetBundle() {
    //    UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("http://www.my-server.com");
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    yield return www.Send();
 
    //    if(www.isNetworkError) {
    //        Debug.Log(www.error);
    //    }
    //    else {
    //        AssetBundle bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
    //    }
    //}

    // Update is called once per frame
    void Update() {
        HandleMovement();


        // Testing Client to Server Commands
        if(isLocalPlayer && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Sending Hello to Server");
            HelloServer();
        }

        if (transform.position.y > 4)
        {
            TooHigh();
        }
    }

    // Client to Server Commands
    [Command]
    void HelloServer()
    {
        Debug.Log("Received Hello from Client");
        ReplyHello();
        helloCount += 1;
    }

    void helloChange(int oldCount, int newCount)
    {
        Debug.Log($"Old Count: {oldCount} Hellos, New Count: {newCount} Hellos");
    }

    // Server to Client Commands
    [ClientRpc]
    void ReplyHello()
    {
        Debug.Log("Received Hello from Server");
    }

    void TooHigh()
    {
        Debug.Log($"Player is too high. Y Position: {transform.position.y}");
    }

    

}
