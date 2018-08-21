using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public List<Pool> pools;
    public Dictionary<string, Transform> poolParent;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolParent = new Dictionary<string, Transform>();

        foreach (Pool pool in pools)
        {
            Transform parent = ParentManager.Instance().GetParent(pool.tag);
            poolParent.Add(pool.tag, parent);

            Queue<GameObject> objPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                //obj.GetComponent<IPooledObject>().objectTag = pool.tag;
                obj.SetActive(false);
                obj.transform.SetParent(parent);
                objPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objPool);
        }
    }

    //生成物体
    public GameObject SpawnObject(string _tag, Vector3 _pos, Quaternion _rot)
    {
        if (!poolDictionary.ContainsKey(_tag))
        {
            Debug.LogWarning("不含此键");
            return null;
        }

        GameObject obj = poolDictionary[_tag].Dequeue();

        obj.SetActive(true);
        obj.transform.position = _pos;
        obj.transform.rotation = _rot;

        if(obj.GetComponent<IPooledObject>() != null)
            obj.GetComponent<IPooledObject>().OnObjectSpawned();

        poolDictionary[_tag].Enqueue(obj);

        return obj;
    }

    public void PutbackObject(GameObject _obj)
    {
        _obj.SetActive(false);

        //string tag = _obj.GetComponent<IPooledObject>().objectTag;

        //poolDictionary[tag].
    }
}
