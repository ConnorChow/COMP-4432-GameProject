using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    public GameObject swarmerPrefab;
    [SerializeField]
    public GameObject bigSwarmerPrefab;

    [SerializeField]
    public float swarmerInterval = 3.5f; //Interchangeable time to spawn enemy
    [SerializeField]
    public float bigSwarmerInterval = 10f; //Interchangeable time to spawn enemy

    public GameObject[] spawnLocations;


    // Start is called before the first frame update
    void Start()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("Spawn");
        Debug.Log(spawnLocations);

        StartCoroutine(spawnEnemy(swarmerInterval,swarmerPrefab)); //Use this to quickly spawn enemy
        StartCoroutine(spawnEnemy(bigSwarmerInterval,bigSwarmerPrefab)); //Use this to spawn a second enemy, preferably a tougher one
    }

    // Update is called once per frame
    public IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        System.Random r = new System.Random();
        int rInt = r.Next(0, spawnLocations.Length);

        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, spawnLocations[rInt].transform.position, spawnLocations[rInt].transform.rotation); //Enemy spawn
        NetworkServer.Spawn(newEnemy);
        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
