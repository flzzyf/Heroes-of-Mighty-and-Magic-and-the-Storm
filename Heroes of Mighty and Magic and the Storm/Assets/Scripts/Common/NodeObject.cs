using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeObjectType { empty, unit, wall, corpse }

public class NodeObject : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int pos;

    [HideInInspector]
    public NodeObjectType nodeObjectType;

    public int player;
}
