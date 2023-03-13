using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Enemy enemyHealth;
    public int damage = 2;
    float timer = 5f;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //Debug.Log($"Time: {timer}");
        if (timer > 0) { timer -= Time.deltaTime; }
        else { Destroy(this.gameObject); }
    }

}
