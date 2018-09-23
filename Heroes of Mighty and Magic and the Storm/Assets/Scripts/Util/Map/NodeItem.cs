using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeItem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int pos;

    public event System.Action<NodeItem> OnMousePress;
    public event System.Action<NodeItem> OnMouseIn;
    public event System.Action<NodeItem> OnMouseOut;

    [HideInInspector]
    public GameObject nodeObject;

    void OnMouseDown()
    {
        OnMousePress(this);
    }

    public virtual void OnMouseEnter()
    {
        OnMouseIn(this);
    }

    void OnMouseExit()
    {
        OnMouseOut(this);
    }
}
