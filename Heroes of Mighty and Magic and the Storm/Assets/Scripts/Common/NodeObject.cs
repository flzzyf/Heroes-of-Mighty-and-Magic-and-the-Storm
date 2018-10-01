using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeObjectType { empty, unit, wall, corpse }

public class NodeObject : MonoBehaviour
{
    [HideInInspector]
    public GameObject nodeUnit;

    [HideInInspector]
    public NodeObjectType nodeObjectType;

    public int player;
}
