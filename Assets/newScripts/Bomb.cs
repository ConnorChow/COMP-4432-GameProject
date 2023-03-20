using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public Enemy enemyHealth;
    public int damage = 10;




    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            enemyHealth.TakeDamage(damage);
            SelfDestruct();
        }

    }

    void SelfDestruct() {
        LandscapeSimulator landscape = GameObject.Find("SceneSimulator").GetComponent<LandscapeSimulator>();
        if (landscape != null) {
            landscape.BurnCellFromV2(new Vector2(transform.position.x, transform.position.y));
        }
        Destroy(gameObject, 5);
    }
}