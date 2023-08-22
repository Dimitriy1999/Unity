using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private List<Transform> objectList;
    private Transform objectWeWantToPool;
    private Transform parent;
    public ObjectPool(int maxCapacity, Transform objectWeWantToPool, Transform parent)
    {
        objectList = new List<Transform>(maxCapacity);
        this.objectWeWantToPool = objectWeWantToPool;
        this.parent = parent;
    }

    public Transform SpawnObject(Vector3 position)
    {
        Transform objectWeSpawned = null;
        bool anyNotActive = false;

        for (int i = 0; i < objectList.Count; i++)
        {
            var item = objectList[i];

            if (item.gameObject.activeInHierarchy) continue;

            item.position = position;
            item.gameObject.SetActive(true);

            return item;
        }
        if (!anyNotActive)
        {
            objectWeSpawned = CreateAndAddObjectToList(position);
        }
        return objectWeSpawned;
    }

    private Transform CreateAndAddObjectToList(Vector3 position)
    {
        var newObject = Object.Instantiate(objectWeWantToPool, position, new Quaternion(0, 0, 0, 0), parent);
        objectList.Add(newObject);
        return newObject;
    }
}
