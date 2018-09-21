using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeItem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int pos;

    public event System.Action<NodeItem> OnMousePress;
    public event System.Action<NodeItem> OnMouseIn;

    void OnMouseDown()
    {
        OnMousePress(this);
    }

    void OnMouseEnter()
    {
        OnMouseIn(this);
    }
}
