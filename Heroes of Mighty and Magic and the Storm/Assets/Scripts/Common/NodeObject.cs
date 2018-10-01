using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeObjectType { empty, unit, wall, corpse }

public class NodeObject : MonoBehaviour
{
    [HideInInspector]
    public NodeItem nodeItem;

    [HideInInspector]
    public NodeObjectType nodeObjectType;

    public int player;
}
