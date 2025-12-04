using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private int poolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary =
        new Dictionary<GameObject, Queue<GameObject>>();

    public GameObject GetObject(GameObject prefab, Vector3 position, Vector3 rotation)
    {
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab); // if all objects of this type are in uise, create a new one.

        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        objectToGet.transform.position = position;
        objectToGet.transform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, 0);
        objectToGet.SetActive(true);

        return objectToGet;
    }

    public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn));
    }

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objectToReturn);
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

        objectToReturn.SetActive(false);

        poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    public void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        GameObject parent = new GameObject(prefab.name + " Holder");

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab, parent);
        }
    }

    private void CreateNewObject(GameObject prefab, GameObject parent = null)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;

        if (parent != null)
        {
            newObject.transform.SetParent(parent.transform, true);
        }

        newObject.SetActive(false);

        poolDictionary[prefab].Enqueue(newObject);
    }
}
