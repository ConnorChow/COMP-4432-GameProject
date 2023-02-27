using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooterEnemy : MonoBehaviour
{
    public static event Action<shooterEnemy> OnEnemyKilled;
    [SerializeField] float health, maxHealth = 3f;
    public float speed;
    public float lineOfSight;
    public float shootingRange;
    public float fireRate = 1f;
    private float nextFireTime;
    public GameObject bullet;
    public GameObject bulletParent;
    private Transform player;

    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if(distanceFromPlayer < lineOfSight && distanceFromPlayer>shootingRange)
        {
                    transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
        }

        else if (distanceFromPlayer <= shootingRange && nextFireTime <Time.time)
        {
            Instantiate(bullet, bulletParent.transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
    }


private void OnDrawGizmoSelected()
{
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, lineOfSight);
    Gizmos.DrawWireSphere(transform.position, shootingRange);
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

