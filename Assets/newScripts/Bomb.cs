using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Enemy enemyHealth;
    public int damage = 10;

    


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }

    }

    void SelfDestruct()
    {
        
        Destroy(gameObject,5);
    }


}