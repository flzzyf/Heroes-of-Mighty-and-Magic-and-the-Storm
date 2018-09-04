using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;

    public static T Instance()
    {
        if (instance == null)
        {
            //寻找现有脚本
            instance = FindObjectOfType<T>();

            if (FindObjectsOfType<T>().Length > 1)
            {
                //超过一个则选择最后一个，即场景中本来就有的
                print("超过一个Singleton!");
                instance = FindObjectsOfType<T>()[FindObjectsOfType<T>().Length - 1];
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
