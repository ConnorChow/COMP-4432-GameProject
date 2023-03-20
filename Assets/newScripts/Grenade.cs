using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private Vector3 targetPos;

    public float speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = GameObject.Find("firePoint").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(speed > 0)
        {
            speed -= Random.Range(.1f, .25f);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (speed < 0)
        {
            speed = 0;
            //Add explosion
        }
    }



    //add this to player movement: 
    //public GameObject grenade;
    
    //if (Input.GetKeyDown(KeyCode.E))
    //{
    //Instantiate(grenade, transform.position, Quaternion.identity);    
    //}
}
