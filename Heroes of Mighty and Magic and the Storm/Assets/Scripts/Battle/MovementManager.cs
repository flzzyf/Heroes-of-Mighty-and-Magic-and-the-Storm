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
    public void MoveObjectAlongPath(Unit _unit, List<NodeItem> _path)
    {
        GameManager.instance.gamePaused = true;

        MoveObjectStart(_unit);

        path = _path;

        StartCoroutine(IEMoveObject(_unit));
    }

    IEnumerator IEMoveObject(Unit _unit)
    {
        float speed = _unit.speed;
        speed = unitSpeedOriginal + unitSpeedMultipler * speed;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPos = path[i].transform.position;

            Vector3 dir = targetPos - _unit.transform.position;
            dir.z = 0;

            //改变单位朝向
            _unit.FaceTarget(path[i].transform.position);

            while (Vector2.Distance(_unit.transform.position, targetPos) > speed * Time.deltaTime)
            {
                _unit.transform.Translate(dir.normalized * speed * Time.deltaTime);

                yield return null;
            }
        }

        StartCoroutine(MoveObjectFinish(_unit, path[path.Count - 1]));
    }
    //开始移动单位
    void MoveObjectStart(Unit _unit)
    {
        moving = true;
        UnitAnimMgr.instance.PlayAnimation(_unit, Anim.Walk);

        if (_unit.type.sound_walk != null)
            StartCoroutine(PlayMoveSound(_unit));
    }

    IEnumerator PlayMoveSound(Unit _unit)
    {
        while (moving)
        {
            if (_unit.type.sound_walk.Length != 0)
                GameManager.instance.PlaySound(_unit.type.sound_walk[Random.Range(0, _unit.type.sound_walk.Length)]);

            yield return new WaitForSeconds(Random.Range(.35f, .45f));
        }
    }
    //移动到目的地后
    IEnumerator MoveObjectFinish(Unit _unit, NodeItem _targetNode)
    {
        GameManager.instance.gamePaused = false;

        UnitAnimMgr.instance.PlayAnimation(_unit, Anim.Walk, false);

        if (_unit.RestoreFacing())
        {
            //需要转身
            yield return new WaitForSeconds(UnitAttackMgr.instance.animTurnbackTime);
        }

        //设置节点上的物体，设置英雄所在位置、节点
        BattleManager.instance.LinkNodeWithUnit(_unit, _targetNode);

        moving = false;
    }

    public void MoveUnitFlying(Unit _unit, NodeItem _node)
    {
        GameManager.instance.gamePaused = true;

        MoveObjectStart(_unit);

        StartCoroutine(MoveUnitFlyingCor(_unit, _node));
    }

    IEnumerator MoveUnitFlyingCor(Unit _unit, NodeItem _node)
    {
        float speed = _unit.speed;
        speed = unitSpeedOriginal + unitSpeedMultipler * speed * flyingSpeedmultipler;

        Vector3 targetPos = _node.transform.position;
        Vector3 dir = targetPos - _unit.transform.position;
        dir.z = 0;

        while (Vector2.Distance(_unit.transform.position, targetPos) > speed * Time.deltaTime)
        {
            _unit.transform.Translate(dir.normalized * speed * Time.deltaTime);

            yield return null;
        }

        _unit.transform.position = targetPos;

        StartCoroutine(MoveObjectFinish(_unit, _node));
    }
}
