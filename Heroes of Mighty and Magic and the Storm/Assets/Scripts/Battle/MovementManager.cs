using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    public float UnitSpeed = 14;
    public float flyingSpeed = 22;

    public static bool moving;
    Transform movingObj;
    List<NodeItem> path;

    //按照路径移动物体
    public void MoveObjectAlongPath(Transform _obj, List<NodeItem> _path)
    {
        GameManager.instance.gamePaused = true;

        movingObj = _obj;
        MoveObjectStart();

        path = _path;

        StartCoroutine(IEMoveObject());
    }

    IEnumerator IEMoveObject()
    {
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 targetPos = path[i].transform.position;

            Vector3 dir = targetPos - movingObj.position;
            dir.z = 0;

            //改变单位朝向
            movingObj.GetComponent<Unit>().FaceTarget(path[i].transform.position);

            while (GetHorizontalDistance(movingObj.position, targetPos) > UnitSpeed * Time.deltaTime)
            {
                movingObj.Translate(dir.normalized * UnitSpeed * Time.deltaTime);

                yield return null;
            }
        }

        MoveObjectFinish(path[path.Count - 1]);
    }
    //开始移动单位
    void MoveObjectStart()
    {
        moving = true;
        movingObj.GetComponent<Unit>().PlayAnimation(Anim.walk);
    }
    //移动到目的地后
    void MoveObjectFinish(NodeItem _targetNode)
    {
        GameManager.instance.gamePaused = false;

        movingObj.GetComponent<Unit>().PlayAnimation(Anim.walk, false);
        movingObj.GetComponent<Unit>().RestoreFacing();

        //设置节点上的物体，设置英雄所在位置、节点
        BattleManager.instance.LinkNodeWithUnit(movingObj.GetComponent<Unit>(), _targetNode);

        moving = false;
    }

    float GetHorizontalDistance(Vector3 _p1, Vector3 _p2)
    {
        _p2.z = _p1.z;
        return Vector3.Distance(_p1, _p2);
    }

    public void MoveUnitFlying(Transform _obj, NodeItem _node)
    {
        GameManager.instance.gamePaused = true;

        movingObj = _obj;
        MoveObjectStart();

        StartCoroutine(MoveUnitFlyingCor(_node));
    }

    IEnumerator MoveUnitFlyingCor(NodeItem _node)
    {
        Vector3 targetPos = _node.transform.position;
        Vector3 dir = targetPos - movingObj.position;
        dir.z = 0;

        while (GetHorizontalDistance(movingObj.position, targetPos) > flyingSpeed * Time.deltaTime)
        {
            movingObj.Translate(dir.normalized * flyingSpeed * Time.deltaTime);

            yield return null;
        }

        MoveObjectFinish(_node);
    }
}
