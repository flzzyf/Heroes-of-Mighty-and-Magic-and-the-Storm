using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    public float unitSpeedOriginal = 14;
    public float unitSpeedMultipler = 0.5f;
    public float flyingSpeedmultipler = 1.5f;

    public static bool moving;
    List<NodeItem> path;

    //按照路径移动物体
    public void MoveObjectAlongPath(Transform _obj, List<NodeItem> _path)
    {
        GameManager.instance.gamePaused = true;

        MoveObjectStart(_obj);

        path = _path;

        StartCoroutine(IEMoveObject(_obj));
    }

    IEnumerator IEMoveObject(Transform _obj)
    {
        float speed = _obj.GetComponent<Unit>().speed;
        speed = unitSpeedOriginal + unitSpeedMultipler * speed;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPos = path[i].transform.position;

            Vector3 dir = targetPos - _obj.position;
            dir.z = 0;

            //改变单位朝向
            _obj.GetComponent<Unit>().FaceTarget(path[i].transform.position);

            while (Vector2.Distance(_obj.position, targetPos) > speed * Time.deltaTime)
            {
                _obj.Translate(dir.normalized * speed * Time.deltaTime);

                yield return null;
            }
        }

        StartCoroutine(MoveObjectFinish(_obj, path[path.Count - 1]));
    }
    //开始移动单位
    void MoveObjectStart(Transform _obj)
    {
        moving = true;
        _obj.GetComponent<Unit>().PlayAnimation(Anim.walk);
    }
    //移动到目的地后
    IEnumerator MoveObjectFinish(Transform _obj, NodeItem _targetNode)
    {
        GameManager.instance.gamePaused = false;

        _obj.GetComponent<Unit>().PlayAnimation(Anim.walk, false);

        if (_obj.GetComponent<Unit>().RestoreFacing())
        {
            //需要转身
            yield return new WaitForSeconds(UnitAttackMgr.instance.animTurnbackTime);
        }

        //设置节点上的物体，设置英雄所在位置、节点
        BattleManager.instance.LinkNodeWithUnit(_obj.GetComponent<Unit>(), _targetNode);

        moving = false;
    }

    public void MoveUnitFlying(Transform _obj, NodeItem _node)
    {
        GameManager.instance.gamePaused = true;

        MoveObjectStart(_obj);

        StartCoroutine(MoveUnitFlyingCor(_obj, _node));
    }

    IEnumerator MoveUnitFlyingCor(Transform _obj, NodeItem _node)
    {
        float speed = _obj.GetComponent<Unit>().speed;
        speed = unitSpeedOriginal + unitSpeedMultipler * speed * flyingSpeedmultipler;

        Vector3 targetPos = _node.transform.position;
        Vector3 dir = targetPos - _obj.position;
        dir.z = 0;

        while (Vector2.Distance(_obj.position, targetPos) > speed * Time.deltaTime)
        {
            _obj.Translate(dir.normalized * speed * Time.deltaTime);

            yield return null;
        }

        _obj.position = targetPos;

        StartCoroutine(MoveObjectFinish(_obj, _node));
    }
}
