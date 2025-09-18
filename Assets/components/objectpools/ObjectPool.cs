using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool<T> : MonoBehaviour where T : PooledObject
{
    public List<T> objectList;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int number;


    public void ReturnAll()
    {
        foreach (T obj in objectList)
        {
            obj.Kill();
        }
    }

    public void SetUp()
    {
        objectList = new List<T>();
        for (int i = 0; i < number; i++)
        {
            GameObject ins = Instantiate(prefab, this.transform);
            T item = ins.GetComponent<T>();
            objectList.Add(item);
        }
        ReturnAll();
    }
    public T GetFirstItem()
    {
        foreach (T obj in objectList)
        {
            if (!obj.used)
            {
                obj.used = true;
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        GameObject ins = Instantiate(prefab, this.transform);
        T item = ins.GetComponent<T>();
        item.used = true;                     // Mark as used immediately
        item.gameObject.SetActive(true);      // Ensure it's active
        objectList.Add(item);
        return item;
    }
}
