using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public Enemy enemyHealth;
    public Player player;

    private Vector2 targetPos;

    public float speed = 5;

    // Start is called before the first frame update
    void Start() {
        targetPos = GameObject.Find("firePoint").transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (speed > 0) {
            speed -= Random.Range(.1f, .25f) * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        } else if (speed <= 0) {
            speed = 0;
            Explode();
        }
    }

    void Explode() {
        //Get the LandscapeSimulator in the screen
        LandscapeSimulator landscape = GameObject.Find("SceneSimulator").GetComponent<LandscapeSimulator>();
        if (landscape != null) {
            //use the function from LandscapeSimulator.cs that allows it to burn the specific tile the bomb stops at
            landscape.BurnCellFromV2(new Vector2(transform.position.x, transform.position.y));
        }


        // Hurt Players and enemies
        


        //Destroy the object
        Destroy(gameObject, 5);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Enemy")
    //    {
    //        enemyHealth.TakeDamage(5);
    //        Destroy(gameObject);
    //    }

    //    if (collision.gameObject.tag == "Player")
    //    {
    //        player.TakeDamage(5);
    //        Destroy(gameObject);
    //    }
    //}



    //add this to player movement: -- Humraj: I added this to the weapon script that connects to the player script.
    //public GameObject grenade;

    //if (Input.GetKeyDown("e"))
    //{
    //Instantiate(grenade, transform.position, Quaternion.identity);    
    //}
}
