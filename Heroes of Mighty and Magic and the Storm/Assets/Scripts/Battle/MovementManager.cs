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
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPos = path[i].transform.position;

            Vector3 dir = targetPos - movingObj.position;
            dir.z = 0;

            //改变单位朝向
            movingObj.GetComponent<Unit>().FaceTarget(path[i].transform.position);

            while (Vector2.Distance(movingObj.position, targetPos) > UnitSpeed * Time.deltaTime)
            {
                movingObj.Translate(dir.normalized * UnitSpeed * Time.deltaTime);

                yield return null;
            }
        }

        StartCoroutine(MoveObjectFinish(path[path.Count - 1]));
    }
    //开始移动单位
    void MoveObjectStart()
    {
        moving = true;
        movingObj.GetComponent<Unit>().PlayAnimation(Anim.walk);
    }
    //移动到目的地后
    IEnumerator MoveObjectFinish(NodeItem _targetNode)
    {
        GameManager.instance.gamePaused = false;

        movingObj.GetComponent<Unit>().PlayAnimation(Anim.walk, false);

        if (movingObj.GetComponent<Unit>().RestoreFacing())
        {
            //需要转身
            yield return new WaitForSeconds(UnitAttackMgr.instance.animTurnbackTime);
        }

        //设置节点上的物体，设置英雄所在位置、节点
        BattleManager.instance.LinkNodeWithUnit(movingObj.GetComponent<Unit>(), _targetNode);

        moving = false;
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

        while (Vector2.Distance(movingObj.position, targetPos) > flyingSpeed * Time.deltaTime)
        {
            movingObj.Translate(dir.normalized * flyingSpeed * Time.deltaTime);

            yield return null;
        }

        movingObj.position = targetPos;

        StartCoroutine(MoveObjectFinish(_node));
    }
}
