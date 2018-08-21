using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPooledObject : MonoBehaviour
{
    [HideInInspector]
    public string objectTag;

    public virtual void OnObjectSpawned() { }
	
}
