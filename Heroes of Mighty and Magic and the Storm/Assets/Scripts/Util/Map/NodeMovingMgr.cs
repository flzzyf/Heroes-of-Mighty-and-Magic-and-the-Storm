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
    public event CommonEvent Event_StopMoving;

    [HideInInspector]
    public bool moving;

    public void MoveObject(GameObject _go, List<NodeItem> _path, float _speed, bool _flying = false)
    {
        moving = true;

        if (!_flying)
            StartCoroutine(MoveObjectCor(_go, _path, _speed));
        else
            StartCoroutine(MoveObjectFlyingCor(_go, _path[_path.Count - 1], _speed));
    }

    public void StopMoving()
    {
        moving = false;

        StopAllCoroutines();

        ClearEvents();

        if (Event_StopMoving != null)
            Event_StopMoving();
    }

    IEnumerator MoveObjectCor(GameObject _go, List<NodeItem> _path, float _speed)
    {
        for (int i = 1; i < _path.Count; i++)
        {
            //朝向下一节点
            //_go.GetComponent<MovableNode>().TowardNextNode(_path[i]);

            if (Event_MovingToNode != null)
                Event_MovingToNode(_path[i]);

            yield return null;

            Vector3 targetPos = _path[i].transform.position;

            Vector3 dir = targetPos - _go.transform.position;
            //dir.z = 0;

            //朝目标移动
            while (Vector3.Distance(_go.transform.position, targetPos) > _speed * Time.deltaTime)
            {
                _go.transform.Translate(dir.normalized * _speed * Time.deltaTime);

                yield return null;
            }

            if (Event_ReachNode != null)
                Event_ReachNode(_path[i]);
        }

        MoveObjectFinish(_path[_path.Count - 1]);
    }

    //
    IEnumerator MoveObjectFlyingCor(GameObject _go, NodeItem _node, float _speed)
    {
        Vector3 targetPos = _node.transform.position;

        yield return null;

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
