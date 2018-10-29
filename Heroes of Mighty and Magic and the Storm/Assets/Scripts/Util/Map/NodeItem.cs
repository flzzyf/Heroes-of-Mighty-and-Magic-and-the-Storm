using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeItem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int pos;

    public event System.Action<NodeItem> OnMousePress;
    public event System.Action<NodeItem> OnMouseIn;
    public event System.Action<NodeItem> OnMouseOut;

    [HideInInspector]
    public NodeObject nodeObject;

    void OnMouseDown()
    {
        //如果是游戏暂停状态则无视点击
        if (GameManager.instance.gamePaused)
            return;

        //鼠标在UI上则无效
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        OnMousePress(this);
    }

    public virtual void OnMouseEnter()
    {
        //如果是游戏暂停状态则无视点击
        if (GameManager.instance.gamePaused)
            return;

        //鼠标在UI上则无效
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        OnMouseIn(this);
    }

    public virtual void OnMouseExit()
    {
        //如果是游戏暂停状态则无视点击
        // if (GameManager.instance.gamePaused)
        //     return;

        //鼠标在UI上则无效
        // if (EventSystem.current.IsPointerOverGameObject())
        //     return;

        OnMouseOut(this);
    }
}
