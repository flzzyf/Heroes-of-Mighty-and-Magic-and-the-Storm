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
            //有超过一个实例
            if (FindObjectsOfType<T>().Length > 1)
            {
                Debug.LogError("有超过一个实例！");
                return trueInstance;
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
