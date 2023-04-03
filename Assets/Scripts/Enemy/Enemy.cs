using Mirror;
using OPS.AntiCheat.Field;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : NetworkBehaviour {
    public static event Action<Enemy> OnEnemyKilled;
    [Header("Initial Data")]
    [SerializeField] ProtectedInt32 health, maxHealth = 4;

    //movement data
    [SerializeField] ProtectedFloat moveSpeed = 3;
    [SerializeField] ProtectedFloat regularSpeed = 3;
    [SerializeField] ProtectedFloat chargeSpeed = 5;
    private Rigidbody2D rb;

    //List of players close to the enemy
    [SerializeField]public readonly SyncHashSet<GameObject> playersDetected = new SyncHashSet<GameObject>();
    //Player that is being targeted
    private GameObject playerTarget = null;
    //Player's script (to track health)
    private Player playerScript = null;

    //Patrol data
    private Vector3 spawnPosition;

    //Data for enemy movement
    private Vector3 target;
    private Vector2 moveDirection;

    [Header("Damage-related Data")]
    //Health Damage Buffer... doesn't actually buffer damage just for showing visual damage
    [SerializeField] ProtectedFloat dmgBufferInterval = 1;
    ProtectedFloat dmgBuffer;
    //Interaction with the fire
    [SerializeField] ProtectedFloat fireCheckInterval = 1;
    ProtectedFloat fireCheck;
    private LandscapeSimulator landscape;

    private PlayerEnemyTracker playerEnemyTracker;

    //Player Sprite
    [SerializeField] SpriteRenderer enemySprite;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        health = maxHealth;

        landscape = GameObject.Find("Landscape").GetComponent<LandscapeSimulator>();

        playerEnemyTracker = GameObject.Find("CharacterTracker").GetComponent<PlayerEnemyTracker>();

        spawnPosition = transform.position;
    }

    private void Update() {
        //HandleMovement();
        AIBehaviour();
        //Movetowards behaviour
        if (moveTo && !reachedLoc) {
            MoveToBehaviour();
        }
        //Damage related behaviour
        if (dmgBuffer > 0) {
            dmgBuffer -= Time.deltaTime;
            enemySprite.color = new Color(1, 1 - dmgBuffer, 1 - dmgBuffer);
        } else {
            enemySprite.color = Color.white;
        }
        if (landscape == null) return;
        if (fireCheck > 0) {
            fireCheck -= Time.deltaTime;
        } else {
            Vector3Int tileLoc = new Vector3Int((int)Mathf.Round(rb.transform.position.x - 0.5f), (int)Mathf.Round(rb.transform.position.y - 0.5f), 0);

            if (landscape.FireGrid.GetTile(tileLoc) != null) {
                TakeDamage(2);
            }
            fireCheck = fireCheckInterval;
        }
    }
    [Header("AI States")]
    //incurred to direct whether to remain idle or to Attack the nearest player
    // 0 = idle
    // 1 = attack
    public ProtectedInt32 behaviourState = 0;
    //incurred only when behaviourState = 1
    // 0 = confront
    // 1 = flank
    // 2 = charge
    // 3 = quick retreat
    public ProtectedInt32 attackState;
    private void AIBehaviour() {
        switch (behaviourState) {
            case 0:
                IdleBehaviour();
                break;
            case 1:
                break;
        }
    }
    [Header("Idle Patrol Data")]
    [SerializeField] ProtectedFloat patrolRadius;
    [SerializeField]ProtectedFloat waitIdlyTime = 2;
    ProtectedFloat idleTimer;
    void IdleBehaviour() {
        if (!moveTo) {
            if (idleTimer > 0) {
                idleTimer -= Time.deltaTime;
            } else {
                MoveTo(FindRandomPointInRadius(spawnPosition, patrolRadius));
                idleTimer = waitIdlyTime;
                moveSpeed = regularSpeed;
            }
        }
        playerTarget = DetectNearestPlayerInRadius();
        if (playerTarget != null) {
            playerScript = playerTarget.GetComponent<Player>();
            behaviourState = 1;
        }
    }

    Vector3 FindRandomPointInRadius(Vector3 center, float radius) {
        Vector3 point = new Vector3(center.x + UnityEngine.Random.Range(-radius, radius), center.y + UnityEngine.Random.Range(-radius, radius), 0);
        return point;
    }
    [Header("Movement Data")]
    //is the enemy currently supposed to be moving somewhere?
    ProtectedBool moveTo;
    //Has the enemy reached their location?
    ProtectedBool reachedLoc;
    public void MoveTo(Vector3 newTarget) {
        moveTo = true;
        target = newTarget;
        reachedLoc = false;
    }

    [SerializeField]ProtectedFloat MoveToTolerance = 0.05f;
    Vector3 lastFrameLoc;
    ProtectedInt32 stuckWarnings = 0;
    private void MoveToBehaviour() {
        if (Vector3.Distance(target, rb.transform.position) < MoveToTolerance) {
            moveTo = false;
            reachedLoc = true;
            rb.velocity = Vector3.zero;
            return;
        } else if (rb.transform.position == lastFrameLoc) {
            if (stuckWarnings >= 100) {
                Debug.Log("Bad Path, ending moveto");
                stuckWarnings = 0;
                moveTo = false;
                reachedLoc = false;
                rb.velocity = Vector3.zero;
                return;
            } else {
                stuckWarnings++;
            }
        } else {
            stuckWarnings = 0;
        }
        Vector3 direction = (target - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
        moveDirection = direction;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
    }

    //Search through the hashset to try and find the nearest player that has entered the enemy's radius
    GameObject DetectNearestPlayerInRadius() {
        if (playersDetected.Count == 0) return null;
        GameObject nearestPlayer = playersDetected.ElementAt(0);
        foreach (GameObject player in playersDetected) {
            float newDistance = Vector3.Distance(transform.position, player.transform.position);
            float currentDistance = Vector3.Distance(transform.position, nearestPlayer.transform.position);
            if (newDistance < currentDistance) {
                nearestPlayer = player;
            }
        }
        return nearestPlayer;
    }

    public void TakeDamage(int damageAmount) {
        health -= damageAmount;

        dmgBuffer = dmgBufferInterval;

        if (health <= 0) {
            Destroy(gameObject);
            OnEnemyKilled?.Invoke(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null) {
            playersDetected.Add(player.gameObject);
            if (playerTarget == null || playerScript == null) {
                playerTarget = player.gameObject;
                playerScript = player;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null) {
            playersDetected.Remove(player.gameObject);
        }
    }
}
