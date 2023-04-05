using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OPS.AntiCheat.Field;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject swarmerPrefab;
    public GameObject bigSwarmerPrefab;

    public ProtectedFloat swarmerInterval = 10f; //Interchangeable time to spawn enemy

    public ProtectedInt32 mapPopulation = 100;

    public GameObject[] spawnLocations;
    public ProtectedFloat spawnLocationsJitter = 20;

    public HashSet<GameObject> enemyList = new HashSet<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("EnemySpawn");
        if (isServer) {
            foreach (GameObject spawnLocation in spawnLocations) {
                spawnLocation.transform.position += new Vector3(Random.Range(-spawnLocationsJitter, spawnLocationsJitter), Random.Range(-spawnLocationsJitter, spawnLocationsJitter), 0);
            }
            for (int i = 0; i < mapPopulation; i++) {
                StartCoroutine(spawnEnemy(swarmerInterval, swarmerPrefab));
            }
        }

        //StartCoroutine(spawnEnemy(swarmerInterval,swarmerPrefab)); //Use this to quickly spawn enemy
        //StartCoroutine(spawnEnemy(bigSwarmerInterval,bigSwarmerPrefab)); //Use this to spawn a second enemy, preferably a tougher one
    }

    // Update is called once per frame
    private void Update()
    {
        /*return;
        int numOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (numOfEnemies >= 10)
        {
            StopAllCoroutines();
        }
        else
        {
            StartCoroutine(spawnEnemy(swarmerInterval, swarmerPrefab)); //Use this to quickly spawn enemy
            StartCoroutine(spawnEnemy(bigSwarmerInterval, bigSwarmerPrefab)); //Use this to spawn a second enemy, preferably a tougher one
        }*/
    }


    public IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        System.Random r = new System.Random();
        int rInt = r.Next(0, spawnLocations.Length);

        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, spawnLocations[rInt].transform.position, spawnLocations[rInt].transform.rotation); //Enemy spawn
        enemyList.Add(newEnemy);
        NetworkServer.Spawn(newEnemy);
        //StartCoroutine(spawnEnemy(interval, enemy));
    }

    void load(HashSet<GameObject> enemies)
    {
        foreach (var enemy in enemies)
        {
            NetworkServer.Spawn(enemy);
        }
    }

    void save()
    {

    }
}
