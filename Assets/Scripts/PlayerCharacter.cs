using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    public float PlayerSpeed = 2.5f;

    Rigidbody2D rb;

    public Camera playerCam;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveY = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(MoveX, MoveY) * PlayerSpeed;

        playerCam.transform.position = new Vector3(rb.position.x, rb.position.y, -10);
    }
}
