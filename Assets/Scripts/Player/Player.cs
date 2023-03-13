using Mirror;
using OPS.AntiCheat.Field;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour {

    // Sync Variable from Server to Client
    [SyncVar(hook = nameof(helloChange))]
    public int helloCount = 0;

    // Player Stats
    public ProtectedInt32 health;
    private ProtectedInt32 maxHealth = Globals.maxHealth;
    private ProtectedFloat playerSpeed = Globals.maxSpeed;
    public ProtectedString playerName;

    // Player Movement
    private Vector2 moveDirection;
    private Vector2 mousePosition;
    //private Transform aimTransform;
    private Rigidbody2D rb;
    public Camera playerCamera;
    public Weapon weapon;

    // Player Skin
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;

    public ProtectedBool isCheater = false;


    private void Awake()
    {
        //aimTransform.transform.Find("Aim");
    }

    // Start is called before the first frame update
    void Start() {
        health = maxHealth;

        // testing skins
        if (helloCount == 1) { spriteRenderer.sprite = newSprite; }

        //StartCoroutine(GetAssetBundle());
        rb = GetComponentInChildren<Rigidbody2D>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update() {
        HandleMovement();
        RotateInDirection0fInput();

        // Testing Client to Server Commands
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Sending Hello to Server");
            HelloServer();
        }

        if (rb.transform.position.y > 4 || rb.transform.position.y > 50 || rb.transform.position.x > 4 || rb.transform.position.x > 50)
        {
            outOfBounds();
        }
    }

    private void FixedUpdate()
    {
       
    }


    // Player Functions
    void HandleMovement()
    {
        // check if not local player
        if (!isLocalPlayer) { return; }

        // handle player movement
        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveY = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(MoveX, MoveY).normalized * playerSpeed;

        playerCamera.transform.position = new Vector3(rb.position.x, rb.position.y, -10);

        moveDirection = new Vector2(MoveX, MoveY).normalized;
        mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E))
        {
            Aim();

            if (Input.GetMouseButtonDown(0))
            {
                weapon.Fire();
            }
        }

    }

    private void RotateInDirection0fInput() {
        // check if not local player
        if (!isLocalPlayer) { return; }

        if (rb.velocity != Vector2.zero && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E))) {
            Quaternion targetRotation = Quaternion.LookRotation(rb.transform.forward, moveDirection);
            Quaternion rotation = Quaternion.RotateTowards(rb.transform.rotation, targetRotation, 10);
            rb.MoveRotation(rotation);
        }
    }

    private void Aim()
    {
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        //aimTransform.LookAt(mousePosition);
        rb.rotation = aimAngle;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) { Destroy(gameObject); }
    }

    public void Heal(int amount)
    {
        if (health !>= 0) { health += amount; }
    }


    // --------------------------


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

    public void setPlayerName(String name)
    {
        playerName = name;
    }

    // Server to Client Commands
    [ClientRpc]
    void ReplyHello()
    {
        Debug.Log("Received Hello from Server");
    }


    // --------------------------


    // Cheat Detection
    [TargetRpc]
    void outOfBounds()
    {
        Debug.Log($"Player is out of bounds. X: {rb.transform.position.x} Y: {rb.transform.position.y}");

        // Add player position adjustment
        Debug.Log($"Moving player back to 0,0");
        rb.transform.position.Set(0, 0, 0);
    }

    [TargetRpc]
    void tooFast()
    {
        Debug.Log($"Player speed is too fast. Speed: {playerSpeed})");

        // Set player speed back to normal
        playerSpeed = Globals.maxSpeed;

        // Raise cheat flag
        isCheater = true;
    }

    [TargetRpc]
    void tooMuchHealth()
    {
        Debug.Log($"Player has too much health. Health: {health})");

        // Set player health back to normal
        health = Globals.maxHealth;

        // Raise cheat flag
        isCheater = true;
    }







    // Extra Code

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
}
