using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject swarmerPrefab;
    [SerializeField]
    private GameObject bigSwarmerPrefab;

    [SerializeField]
    private float swarmerInterval = 3.5f; //Interchangeable time to spawn enemy
    [SerializeField]
    private float bigSwarmerInterval = 10f; //Interchangeable time to spawn enemy


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(swarmerInterval,swarmerPrefab)); //Use this to quickly spawn enemy
        StartCoroutine(spawnEnemy(bigSwarmerInterval,bigSwarmerPrefab)); //Use this to spawn a second enemy, preferably a tougher one
    }

    // Update is called once per frame
    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-5f,5), Random.Range(-6f, 6f),0),Quaternion.identity); //Enemy spawn
        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
