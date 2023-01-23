using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

class SpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public float SpawnRate;
}

class SpawnerBaker : Baker<SpawnerAuthoring> {
    public override void Bake(SpawnerAuthoring authoring)
    {
        AddComponent(new Spawner
        {
            Prefab = GetEntity(authoring.Prefab),
            SpawnPosition = authoring.transform.position + new Vector3(UnityEngine.Random.Range(-100.0f, 100.0f), 0, UnityEngine.Random.Range(-100.0f, 100.0f)),
            NextSpawnTime = 0.0f,
            SpawnRate = authoring.SpawnRate
        });
    }
}