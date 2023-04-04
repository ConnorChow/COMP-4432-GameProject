using Mirror;
using OPS.AntiCheat.Field;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {

    // Sync Variable from Server to Client
    [SyncVar(hook = nameof(helloChange))]
    public int helloCount = 0;

    public readonly myNetworkManager networkManager = new myNetworkManager();

    // Player Stats
    public ProtectedInt32 health;
    private ProtectedInt32 maxHealth = Globals.maxHealth;
    private ProtectedFloat playerSpeed = Globals.maxSpeed;
    public ProtectedString playerName;
    public ProtectedBool isDead = false;
    public TMP_Text ipAddress = null;

    // Player Movement
    private Vector2 moveDirection;
    public Rigidbody2D rb;
    public Camera playerCamera;
    public Weapon weapon;

    //Timer for the player's bow mechanic
    [SerializeField] ProtectedFloat bowCooldown = 1;
    ProtectedFloat bowTimer;
    ProtectedBool canFireArrow = false;
    [SerializeField] Slider bowCooldownSlider;

    //Timer for the player's bomb mechanic
    [SerializeField] ProtectedFloat bombCooldown = 30;
    ProtectedFloat bombTimer;
    ProtectedBool canFireBomb = false;
    [SerializeField] Slider bombCooldownSlider;

    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject reviveText;
    [SerializeField] private GameObject reviveTimer;
    [SerializeField] private GameObject deathOverlay;

    public GameObject playerMap;
    [SerializeField] private GameObject pauseMenu;
    //Pause Menu Buttons for resuming the game and quitting the game
    [SerializeField] Button resumeButton;
    [SerializeField] Button quitButton;
    private ProtectedBool paused = false;

    public ProtectedBool isCheater = false;

    // Spawn Locations
    public GameObject[] spawnLocations;
    int spawnLocationChoice = 0;

    //Health damage buffer (also a timer for checking burns on the player)
    [SerializeField] ProtectedFloat dmgBufferInterval = 1;
    ProtectedFloat dmgBuffer;
    [SerializeField] ProtectedFloat fireCheckInterval = 1;
    ProtectedFloat fireCheck;
    private LandscapeSimulator landscape;
    //Player Sprite... called to modify colours on damage
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] Slider healthSlider;

    [SerializeField] Sprite deadSprite;
    [SerializeField] Sprite aliveSprite;

    // Start is called before the first frame update
    void Start() {
        health = maxHealth;
        playerSprite.sprite = aliveSprite;

        spawnLocations = GameObject.FindGameObjectsWithTag("Spawn");

        foreach (var item in spawnLocations)
        {
            NetworkManager.startPositions.Add(item.transform);
        }

        //System.Random r = new System.Random();
        //int rInt = r.Next(0, spawnLocations.Length);
        //spawnLocationChoice = rInt;


        // testing skins
        //if (helloCount == 1) { spriteRenderer.sprite = newSprite; }

        reviveText = GameObject.FindGameObjectWithTag("Revive");
        reviveTimer = GameObject.FindGameObjectWithTag("Timer");
        deathOverlay = GameObject.FindGameObjectWithTag("DeathOverlay");

        rb = GetComponentInChildren<Rigidbody2D>();

        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(Quit);


        playerCamera = GetComponentInChildren<Camera>();

        landscape = GameObject.Find("Landscape").GetComponent<LandscapeSimulator>();
        //Debug.Log($"Spawn location choice: {rInt} -> {spawnLocations[rInt].transform}");
        //Debug.Log($"Player position: {rb.transform}");

        // Set player to random spawn location
        //rb.transform.position.Set(spawnLocations[spawnLocationChoice].transform.position.x, spawnLocations[spawnLocationChoice].transform.position.y, spawnLocations[spawnLocationChoice].transform.position.z);
    }

    // Update is called once per frame
    void Update() {
        HandleMovement();
        RotateInDirection0fInput();

        healthSlider.value = (float)health / (float)maxHealth;

        // Testing Client to Server Commands
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.X)) {
            Debug.Log("Sending Hello to Server");
            HelloServer();
        }

        if (!isLocalPlayer) {
            playerCamera.gameObject.SetActive(false);
        }

        //Damage related behaviour
        if (dmgBuffer > 0) {
            dmgBuffer -= Time.deltaTime;
            playerSprite.color = new Color(1, 1 - dmgBuffer, 1 - dmgBuffer);
        } else {
            playerSprite.color = Color.white;
        }

        if (fireCheck > 0) {
            fireCheck -= Time.deltaTime;
        } else {
            Vector3Int tileLoc = new Vector3Int((int)Mathf.Round(rb.transform.position.x - 0.5f), (int)Mathf.Round(rb.transform.position.y - 0.5f), 0);

            if (landscape.FireGrid.GetTile(tileLoc) != null)
            {
                TakeDamage(2);
            }
            fireCheck = fireCheckInterval;
        }

        if (!isLocalPlayer) {
            playerHUD.SetActive(false);
            pauseMenu.SetActive(false);
            return;
        }

        if (!isDead)
        {
            reviveText.SetActive(false);
            reviveTimer.SetActive(false);
            deathOverlay.SetActive(false);
        }
        //Display the cooldown on objects
        if (bowTimer > 0) {
            bowTimer -= Time.deltaTime;
            bowCooldownSlider.value = 1 - (bowTimer / bowCooldown);
        } else {
            bowCooldownSlider.value = 1;
        }
        if (bombTimer > 0) {
            bombTimer -= Time.deltaTime;
            bombCooldownSlider.value = 1 - (bombTimer / bombCooldown);
        } else {
            bombCooldownSlider.value = 1;
        }

        //Handle inputs from the player
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E)) && !paused && bombTimer <= 0) {
            bombTimer = bombCooldown;
            CmdFireGrenade();
        }
        if (Input.GetMouseButtonDown(0) && !paused && bowTimer <= 0) {
            bowTimer = bowCooldown;
            CmdFireArrow();
        }

        // Pausing
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) Resume();
            else Pause();
        }

        // Suicide
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    TakeDamage(10);
        //}

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(revive(this));
        }

        rb.angularVelocity = 0;

        updateIP();
    }

    // Player Functions
    void HandleMovement() {
        // check if not local player
        if (!isLocalPlayer) { return; }

        if (isDead) { return; }

        // handle player movement
        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveY = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(MoveX, MoveY).normalized * playerSpeed;

        // Fix the bug where player starts randomly spinning
        if (MoveX == 0 && MoveY == 0 && rb.velocity.normalized != new Vector2(0, 0)) {
            rb.velocity = new Vector2(0, 0);
            rb.rotation = 0f;
        }

        //this.gameObject.transform.position = new Vector2(rb.position.x, rb.position.y);
        playerCamera.transform.position = new Vector3(rb.position.x, rb.position.y, -10);
    }

    private void RotateInDirection0fInput() {
        // check if not local player
        if (!isLocalPlayer) { return; }
        Aim(); 
    }

    private void Aim() {
        if (isDead) { return; }
        // Mouse Based Rotation
        Vector3 mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        Quaternion targetRotation = Quaternion.LookRotation(rb.transform.forward, mousePos - rb.transform.position);
        Quaternion rotation = Quaternion.RotateTowards(rb.transform.rotation, targetRotation, 1000 * Time.fixedDeltaTime);
        rb.MoveRotation(rotation);
    }

    [Command(requiresAuthority = false)]
    public void TakeDamage(int amount) {
        if (dmgBuffer > 0) return;
        ApplyDamage(amount);
    }

    [ClientRpc]
    void ApplyDamage(int amount) {
        health -= amount;

        dmgBuffer = dmgBufferInterval;

        RequestToCry();

        healthSlider.value = (float)health / (float)maxHealth;

        if (health <= 0) {
            Die();
        }
    }
    public void Die() {
        bombTimer = 0;
        bowTimer = 0;
        isDead = true;
        health = 0;
        reviveText.SetActive(true);
        deathOverlay.SetActive(true);
        RequestKillPlayer();
    }

    [Command(requiresAuthority = false)]
    public void RequestKillPlayer() {
        KillPlayer();
    }
    [ClientRpc]
    public void KillPlayer() {
        Debug.Log("Kill Player");
        playerSprite.sprite = deadSprite;
        rb.simulated = false;
        //playerSprite.gameObject.SetActive(false);
    }

    [Command(requiresAuthority =false)]
    void RequestToCry() {
        YouCanCry();
    }

    [ClientRpc]
    void YouCanCry() {
        playerAudioSource.Play();
    }

    [Command]
    void RequestToHeal(int amount)
    {
        Heal(amount);
    }

    [ClientRpc]
    public void Heal(int amount) {
        if (health < Globals.maxHealth) { health += amount; }
    }

    public void OnApplicationPause(bool pause) {

    }

    [Client]
    public void Pause() {
        if (!paused) {
            paused = true;
            playerHUD.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(true);
            Debug.Log("Paused");
            //OnApplicationPause(paused);
            Time.timeScale = 0;
        }
    }

    [Client]
    public void Resume() {
        if (paused) {
            paused = false;
            pauseMenu.gameObject.SetActive(false);
            playerHUD.gameObject.SetActive(true);
            Debug.Log("Resumed");
            Time.timeScale = 1;
        }
    }

    public void Quit() {
        if (isServer) { NetworkServer.DisconnectAll(); }

        if (isClient) { NetworkClient.Disconnect(); }
    }

    public void updateIP() {
        ipAddress.text = ("IP: " + networkManager.GetLocalIPv4());
    }

    [Server]
    void allDead()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int deadCount = 0;

        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].gameObject.GetComponent<Player>().health == 0) {
                deadCount += 1;
            }

            if (deadCount == players.Length) {
                // Show game over screen
            }
        }
    }

   

    [Command]
    void respawn()
    {

    }

    [TargetRpc]
    void respawnPlayer()
    {
        NetworkClient.localPlayer.transform.position.Set(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        playerSprite.gameObject.SetActive(true);
        health = Globals.maxHealth;
    }

    [Command]
    void requestRestart()
    {
        // Check if players are ready to restart
        if (NetworkClient.ready)
        {
            restartWorld();
        }
    }

    [Server]
    void restartWorld()
    {

    }


    // Revive
    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null && player.health <= 0)
        {
            Debug.Log("Start Reviving");
            StartCoroutine(revive(player));
        }
    }

    private void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            Debug.Log("Stop Reviving");
            StopCoroutine(revive(player));
        }
    }

    public IEnumerator revive(Player player)
    {
        reviveText.SetActive(false);
        reviveTimer.SetActive(true);
        while (player.health < 10)
        {
            reviveTimer.GetComponent<TMP_Text>().text = $"{player.health * 10 + 10}%\nRevived";
            Debug.Log($"Reviving: {player.health * 10}%");
            player.RequestToHeal(1);
            yield return new WaitForSecondsRealtime(1f);
        }

        if (player.health == Globals.maxHealth)
        {
            reviveTimer.SetActive(false);
            deathOverlay.SetActive(true);
            Debug.Log("Revived");
            player.isDead = false;
            rb.simulated = true;
            playerSprite.sprite = aliveSprite;
            //playerSprite.gameObject.SetActive(true);
        }
    }

    // --------------------------


    // Client to Server Commands
    [Command]
    void HelloServer() {
        Debug.Log("Received Hello from Client");
        ReplyHello();
        helloCount += 1;
    }

    void helloChange(int oldCount, int newCount) {
        Debug.Log($"Old Count: {oldCount} Hellos, New Count: {newCount} Hellos");
    }

    public void setPlayerName(String name) {
        playerName = name;
    }

    // Server to Client Commands
    [ClientRpc]
    void ReplyHello() {
        Debug.Log("Received Hello from Server");
    }


    // this is called by clients on the server
    [Command]
    void CmdFireArrow() {
        if (playerSprite.gameObject.activeSelf == false) { return; }
        GameObject projectile = Instantiate(weapon.arrow, weapon.firePoint.position, weapon.firePoint.rotation);
        projectile.GetComponent<Rigidbody2D>().AddForce(weapon.firePoint.up * weapon.fireForce, ForceMode2D.Impulse);
        NetworkServer.Spawn(projectile); // Commenting this causes a error --- Need to fix double shots

        RpcOnFire(projectile);
    }

    [Command]
    void CmdFireGrenade() {
        if (playerSprite.gameObject.activeSelf == false) { return; }
        GameObject projectile = Instantiate(weapon.grenade, weapon.firePoint.position, weapon.firePoint.rotation);
        projectile.GetComponent<Rigidbody2D>().AddForce(weapon.firePoint.up * weapon.fireForce, ForceMode2D.Impulse);
        NetworkServer.Spawn(projectile); // Commenting this causes a error --- Need to fix double shots

        RpcOnFire(projectile);
    }

    // this is called on the player that fired for all observers
    [ClientRpc]
    void RpcOnFire(GameObject projectile) {
        NetworkServer.Spawn(projectile); // Sync projectile to clients
        //animator.SetTrigger("Shoot");
    }

    // --------------------------


    // Cheat Detection
    [TargetRpc]
    void outOfBounds() {
        Debug.Log($"Player is out of bounds. X: {rb.transform.position.x} Y: {rb.transform.position.y}");

        // Add player position adjustment
        Debug.Log($"Moving player back to 0,0");
        rb.transform.position.Set(0, 0, 0);
    }

    [TargetRpc]
    void tooFast() {
        Debug.Log($"Player speed is too fast. Speed: {playerSpeed})");

        // Set player speed back to normal
        playerSpeed = Globals.maxSpeed;

        // Raise cheat flag
        isCheater = true;
    }

    [TargetRpc]
    void tooMuchHealth() {
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
