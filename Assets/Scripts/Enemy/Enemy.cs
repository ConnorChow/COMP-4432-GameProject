using Mirror;
using OPS.AntiCheat.Field;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NetworkBehaviour {
    public static event Action<Enemy> OnEnemyKilled;
    [SerializeField] ProtectedInt32 health, maxHealth = 4;

    //movement data
    [SerializeField] ProtectedFloat moveSpeed = 4;
    [SerializeField] ProtectedFloat chargeSpeed = 5;
    private Rigidbody2D rb;

    //Player that is being targeted
    private GameObject playerTarget;
    //Player's script (to track health)
    private Player playerScript;

    private Vector3 target;
    private Vector2 moveDirection;

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
    //incurred to direct whether to remain idle or to Attack the nearest player
    // 0 = idle
    // 1 = attack
    public ProtectedInt32 behaviourState;
    //incurred only when behaviourState = 1
    // 0 = confront
    // 1 = flank
    // 2 = charge
    public ProtectedInt32 attackState;
    private void AIBehaviour() {
        switch (behaviourState) {
            case 0:
                break;
            default:
                break;
        }
    }

    ProtectedFloat waitIdlyTime;
    void IdleBehaviour() {

    }

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
        } else if (rb.transform.position == lastFrameLoc) {
            if (stuckWarnings > 100) {
                stuckWarnings = 0;
                moveTo = false;
                reachedLoc = false;
            }
        }
        Vector3 direction = (target - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
        moveDirection = direction;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
    }

    /*void HandleMovement() { 
        if (target) {
            Vector3 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
            moveDirection = direction;
        }

        if (target) {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }

    }*/

    public void TakeDamage(int damageAmount) {
        health -= damageAmount;

        dmgBuffer = dmgBufferInterval;

        if (health <= 0) {
            Destroy(gameObject);
            OnEnemyKilled?.Invoke(this);
        }
    }


}
