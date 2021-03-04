using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoSingleton<ObjectPooler>
{
    Dictionary<Object, Queue<Object>> Pools = new Dictionary<Object, Queue<Object>>();

    //objPrefab is the key to our dictionary
    public void InitializePool(Object objPrefab, int Quantity )
    {
        //checks if the pool already contains the object we are passing it
        if (Pools.ContainsKey(objPrefab))
            return;

        //define our new queue
        Queue<Object> queue = new Queue<Object>();

        //create a loop for the amount of objects we want to spawn
        for(int i = 0; i < Quantity; ++i)
        {
            GameObject temp = Instantiate(objPrefab) as GameObject;
            
            SetActive(temp, false);

            queue.Enqueue(temp);
        }

        //setting our pools queue equal to the queue we just created
        Pools[objPrefab] = queue;
    }
    static void SetActive(Object obj, bool active)
    {
        GameObject go = null;

        if (obj is Component component)
        {
            go = component.gameObject;
        }
        else
        {
            go = obj as GameObject;
        }

        go.SetActive(active);
    }

    public T GetInstance<T>(Object prefab) where T : Object
    {
        Queue<Object> queue;
        if (Pools.TryGetValue(prefab, out queue))
        {
            Object obj;

            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
            }
            else
            {
                obj = Instantiate(prefab);
            }

            SetActive(obj, true);
            queue.Enqueue(obj);

            return obj as T;
        }

        UnityEngine.Debug.LogError("No pool was init with this prefab");
        return null;
    }


    public GameObject GetFromPool(Object objPrefab, Vector3 position, Quaternion rotation)
    {
        //if we dont have a key to our objPrefab
        if (!Pools.ContainsKey(objPrefab)) 
        {
            Debug.LogError("cannot find object with key: " + objPrefab);
            return null;
        }
        //grab our item from the list and manipulate its pos/rot
        //GameObject temp = Pools[objPrefab].Dequeue() as GameObject;
        GameObject temp = GetInstance<GameObject>(objPrefab);

        temp.transform.position = position;
        temp.transform.rotation = rotation;

        SetActive(temp, false);
        SetActive(temp, true);

        //add our new and improved item back to the queue
        //Pools[objPrefab].Enqueue(temp);

        return temp;
    }
}
