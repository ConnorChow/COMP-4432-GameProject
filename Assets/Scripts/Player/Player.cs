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

    [SyncVar(hook = nameof(playerCountChanged))]
    public int playerCount = 0;

    // Player Stats
    public ProtectedInt32 health;
    private ProtectedInt32 maxHealth = Globals.maxHealth;
    private ProtectedFloat playerSpeed = Globals.maxSpeed;
    public ProtectedString playerName;

    // Player Movement
    private Vector2 moveDirection;
    private Transform aimTransform;
    private Rigidbody2D rb;
    public Camera playerCamera;
    public Weapon weapon;

    // Player Skin
    //public SpriteRenderer spriteRenderer;
    //public Sprite newSprite;

    private GameObject playerHUD;
    private GameObject pauseMenu;
    private ProtectedBool paused = false;

    public ProtectedBool isCheater = false;

    // Spawn Locations
    Vector3[] spawnLocations = { new Vector3(0, 0, 0), new Vector3(15, 0, 0), new Vector3(-15, 0, 0) };
    int spawnLocationChoice = 0;


    // Start is called before the first frame update
    void Start() {
        health = maxHealth;

        // testing skins
        //if (helloCount == 1) { spriteRenderer.sprite = newSprite; }

        playerCount++;

        rb = GetComponentInChildren<Rigidbody2D>();
        playerCamera = GetComponentInChildren<Camera>();

        rb.transform.position.Set(spawnLocations[spawnLocationChoice].x, spawnLocations[spawnLocationChoice].y, spawnLocations[spawnLocationChoice].z);

        playerHUD = GameObject.FindGameObjectWithTag("HUD");
        pauseMenu = GameObject.FindGameObjectWithTag("Pause");
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

        try
        {
            if (rb.transform.position.y > 50 || rb.transform.position.y > 50 || rb.transform.position.x > 50 || rb.transform.position.x > 50)
            {
                outOfBounds();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
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

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E))
        {
            weapon.FireGrenade();
        }
        if (Input.GetMouseButtonDown(0))
        {
            weapon.FireArrow();
        }

        // Pausing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) { Resume(); }
            if (!paused) { Pause(); }
        }

    }

    private void RotateInDirection0fInput() {
        // check if not local player
        if (!isLocalPlayer) { return; }

        // Input Based Mouse Rotation
        //if (Input.GetMouseButtonDown(1))
        //{
            // Mouse Based Rotation
            Aim();
        //}

        // Movement Based Rotation
        //if (rb.velocity != Vector2.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(rb.transform.forward, moveDirection);
        //    Quaternion rotation = Quaternion.RotateTowards(rb.transform.rotation, targetRotation, 5);
        //    rb.MoveRotation(rotation);
        //}
    }

    private void Aim()
    {
        // Mouse Based Rotation
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Quaternion targetRotation = Quaternion.LookRotation(rb.transform.forward, mousePos - transform.position);
        Quaternion rotation = Quaternion.RotateTowards(rb.transform.rotation, targetRotation, 10);
        rb.MoveRotation(rotation);
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

    public void OnApplicationPause(bool pause)
    {
        
    }

    public void Pause()
    {
        if (!paused)
        {
            playerHUD.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(true);
            Debug.Log("Paused");
            OnApplicationPause(paused);
        }
        //else
        //{
        //    Resume();
        //}
    }

    public void Resume()
    {
        if (paused)
        {
            pauseMenu.gameObject.SetActive(false);
            playerHUD.gameObject.SetActive(true);
            Debug.Log("Resumed");
        }
        //else
        //{
        //    Pause();
        //}
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

    void playerCountChanged(int oldCOunt, int newCount)
    {
        if (newCount == 2) { spawnLocationChoice = 1; }
        if (newCount == 3) { spawnLocationChoice = 2; }
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

    void SpawnEnemies()
    {

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
