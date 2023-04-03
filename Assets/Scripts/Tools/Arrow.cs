using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Arrow : MonoBehaviour
{
    public Enemy enemyHealth;
    public Player player;
    public int damage = 2;

    // 5s timer
    float timer = 5f;


    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    */
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 0.25f);
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 0.25f);
        }
    }

    private void Update()
    {
        //Debug.Log($"Time: {timer}");
        if (timer > 0) { timer -= Time.deltaTime; }
        else
        {
            try
            {
                NetworkServer.Destroy(this.gameObject);
            } catch (Exception e)
            {
                Debug.Log("Arrow could not be destroyed over network. Destroying locally.\n" + e);
                Destroy(this.gameObject);
            }
            
        }
    }

}
