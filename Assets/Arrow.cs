using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Enemy enemyHealth;
    public int damage = 2;

    


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }


}
