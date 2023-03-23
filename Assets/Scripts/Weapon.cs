using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : MonoBehaviour
{

    public GameObject arrow;

    public GameObject grenade;

    public Transform firePoint;

    public float fireForce;

    public void FireArrow()
    {
        GameObject projectile = Instantiate(arrow, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        //Debug.Log("Spawning arrow projectile");
        //NetworkServer.Spawn(projectile);
        //Debug.Log("Spawned arrow projectile");
        rb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
    }

    public void FireGrenade()
    {
        GameObject projectile = Instantiate(grenade, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        //Debug.Log("Spawning grenade projectile");
        //NetworkServer.Spawn(projectile);
        //Debug.Log("Spawned grenade projectile");
        rb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
    }
}
