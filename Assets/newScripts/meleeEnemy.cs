using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeEnemy : MonoBehaviour
{
    public static event Action<meleeEnemy> OnEnemyKilled;
    [SerializeField] float health, maxHealth = 3f;
    public float speed;
    public float lineOfSight;
    private Transform player;

    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if(distanceFromPlayer < lineOfSight)
        {
                    transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
        }
    }


private void OnDrawGizmoSelected()
{
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, lineOfSight);
}

public void TakeDamage(float damageAmount)
    {
        Debug.Log($"Damage Amount: {damageAmount}");
        health -= damageAmount;
        Debug.Log($"Health is now:{health}");

        if (health <= 0)
        {
            GetComponent<lootBag>().InstantiateLoot(transform.position);
            Destroy(gameObject);
            OnEnemyKilled?.Invoke(this);
        }
    }

}
