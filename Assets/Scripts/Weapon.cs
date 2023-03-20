using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        rb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
    }

    public void FireGrenade()
    {
        //GameObject projectile =
            Instantiate(grenade, firePoint.position, firePoint.rotation);
        //Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        //rb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
    }
}
