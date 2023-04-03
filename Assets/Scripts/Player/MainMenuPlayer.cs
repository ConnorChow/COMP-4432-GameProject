using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MainMenuPlayer : MonoBehaviour {
    public Rigidbody2D rb;

    public Weapon weapon;

    private Vector2 moveDirection;

    private Vector2 mousePosition;

    //public GameObject PointA;
    //public GameObject PointB;

    // Update is called once per frame
    void Start() {
        //GameObject weaponObject = new GameObject("Weapon");
        //weapon = weaponObject.AddComponent<Weapon>();
    }

    void Update() {
        RotateInDirection0fInput();

        if (Input.GetMouseButtonDown(0)) {
            weapon.FireArrow();
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            weapon.FireGrenade();
        }

    }

    private void RotateInDirection0fInput() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Quaternion targetRotation = Quaternion.LookRotation(rb.transform.forward, mousePos - transform.position);
        Quaternion rotation = Quaternion.RotateTowards(rb.transform.rotation, targetRotation, 5);
        rb.MoveRotation(rotation);
    }
}
