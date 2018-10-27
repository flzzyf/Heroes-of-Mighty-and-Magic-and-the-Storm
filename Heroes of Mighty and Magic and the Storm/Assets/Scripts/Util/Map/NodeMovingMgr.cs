using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMovingMgr : Singleton<NodeMovingMgr>
{
    public delegate void CommonEvent();
    public delegate void NodeEvent(NodeItem _node);
    public event NodeEvent Event_MovingToNode;
    public event NodeEvent Event_ReachNode;
    public event NodeEvent Event_ReachTarget;
    public event CommonEvent Event_StartMoving;
    public event CommonEvent Event_StopMoving;

    [HideInInspector]
    public bool moving;

    public void MoveObject(GameObject _go, List<NodeItem> _path, float _speed, MapCoord _coord)
    {
        StartMoving();

        StartCoroutine(MoveObjectCor(_go, _path, _speed, _coord));
    }

    public void MoveObjectFlying(GameObject _go, NodeItem _node, float _speed, MapCoord _coord)
    {
        StartMoving();

        StartCoroutine(MoveObjectFlyingCor(_go, _node, _speed, _coord));
    }

    void StartMoving()
    {
        moving = true;

        if (Event_StartMoving != null)
            Event_StartMoving();
    }

    public void StopMoving()
    {
        moving = false;

        StopAllCoroutines();

        ClearEvents();

        if (Event_StopMoving != null)
            Event_StopMoving();
    }

    IEnumerator MoveObjectCor(GameObject _go, List<NodeItem> _path, float _speed, MapCoord _coord)
    {
        for (int i = 1; i < _path.Count; i++)
        {
            //朝向下一节点
            //_go.GetComponent<MovableNode>().TowardNextNode(_path[i]);

            if (Event_MovingToNode != null)
                Event_MovingToNode(_path[i]);

            Vector3 targetPos = _path[i].transform.position;

            Vector3 dir = GetCoordDir(_coord, targetPos, _go.transform.position);

            //朝目标移动
            while (GetCoordDir(_coord, targetPos, _go.transform.position).magnitude > _speed * Time.deltaTime)
            {
                yield return null;

                _go.transform.Translate(dir.normalized * _speed * Time.deltaTime);
            }

            if (Event_ReachNode != null)
                Event_ReachNode(_path[i]);
        }

        MoveObjectFinish(_path[_path.Count - 1]);
    }

    Vector3 GetCoordDir(MapCoord _coord, Vector3 _origin, Vector3 _target)
    {
        Vector3 dir = _origin - _target;
        if (_coord == MapCoord.xz)
            dir.y = 0;
        else
            dir.z = 0;

        return dir;
    }

    IEnumerator MoveObjectFlyingCor(GameObject _go, NodeItem _node, float _speed, MapCoord _coord)
    {
        Vector3 targetPos = _node.transform.position;
        Vector3 dir = GetCoordDir(_coord, targetPos, _go.transform.position);

        if (Event_MovingToNode != null)
            Event_MovingToNode(_node);

        while (GetCoordDir(_coord, targetPos, _go.transform.position).magnitude > _speed * Time.deltaTime)
        {
            _go.transform.Translate(dir.normalized * _speed * Time.deltaTime);

            yield return null;
        }

        MoveObjectFinish(_node);
    }

    void MoveObjectFinish(NodeItem _node)
    {
        StopMoving();

        if (Event_ReachTarget != null)
            Event_ReachTarget(_node);
    }

    void ClearEvents()
    {
        Event_MovingToNode = null;
        Event_ReachNode = null;
        Event_ReachTarget = null;
    }
}

public interface MovableNode
{
    void TowardNextNode(NodeItem _node);
}
