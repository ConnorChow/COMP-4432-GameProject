using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
        }
    }


    public void TakeDamage(int amount)
    {
        health -= amount;
        if(health <=0)
        {
            Destroy(gameObject);
        }
        healthBar.SetHealth(health);
    }
}
