using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] private Transform ObjectToSpawn;
    ObjectPool objectPool;
    Transform test;
    private void Awake()
    {
        objectPool = new ObjectPool(10, ObjectToSpawn, transform);

    }
    public void SpawnObject()
    {
        var spawnedObject = objectPool.SpawnObject(transform.position);
        
        if (!spawnedObject.TryGetComponent<Destructable>(out var destructableObject)) return;

        destructableObject.ResetObjectData();
    }


    public void SpawnObjectTwo()
    {
        if (test == null)
        {
            test = objectPool.SpawnObject(transform.position);
        }
        else
        {
            test.transform.position = transform.position;
            var destructableObject = test.GetComponent<Destructable>();
            destructableObject.ResetObjectData();
        }
    }
}
