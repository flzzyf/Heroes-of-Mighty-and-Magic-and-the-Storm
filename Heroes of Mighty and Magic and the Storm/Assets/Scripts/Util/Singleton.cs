using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T trueInstance = null;

    public static T instance
    {
        get
        {
            return Instance();
        }
    }

    public static T Instance()
    {
        if (trueInstance == null)
        {
            trueInstance = FindObjectOfType<T>();
            //有超过一个实例，取最后一个，即最新的
            if (FindObjectsOfType<T>().Length > 1)
            {
                return FindObjectsOfType<T>()[FindObjectsOfType<T>().Length - 1];
            }
            //不存在实例
            if (trueInstance == null)
            {
                string trueInstanceName = typeof(T).Name;
                Debug.LogError(trueInstanceName + "尚无实例！");
            }
        }

        return trueInstance;
    }

    protected virtual void OnDestroy()
    {
        trueInstance = null;
    }
}
