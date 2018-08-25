using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeItem : MonoBehaviour 
{
    [HideInInspector]
    public Vector2Int pos;

    public SpriteRenderer gfx;

    //被鼠标高亮
    [HideInInspector]
    public bool highlighted;
    [HideInInspector]
    public bool isPath;

    protected Color color_normal;



    public virtual void UpdateStatus()
    {

    }

}
