using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T instance;

    public static T Instance()
    {
        if (instance == null)
        {
            //寻找现有脚本
            instance = FindObjectOfType<T>();

            if (FindObjectsOfType<T>().Length > 1)
            {
                throw new Exception("超过一个Singleton!");
            }
            //没有现有脚本
            if (instance == null)
            {
                string instanceName = typeof(T).Name;
                print(typeof(T).Name + "没有现有脚本");
            }
        }

        return instance;
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
}
