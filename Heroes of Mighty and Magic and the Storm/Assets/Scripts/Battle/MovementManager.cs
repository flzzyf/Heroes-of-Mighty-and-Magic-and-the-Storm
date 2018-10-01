using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    public float UnitSpeed = 14;

    public static bool moving;
    Transform movingObj;
    List<NodeItem> path;

    //按照路径移动物体
    public void MoveObjectAlongPath(Transform _obj, List<NodeItem> _path)
    {
        GameManager.instance.gamePaused = true;

        movingObj = _obj;
        path = _path;

        moving = true;
        _obj.GetComponent<Unit>().PlayAnimation(Anim.walk);

        StartCoroutine(IEMoveObject());
    }

    IEnumerator IEMoveObject()
    {
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 targetPos = path[i].transform.position;

            Vector3 dir = targetPos - movingObj.position;
            dir.z = 0;
            while (GetHorizontalDistance(movingObj.position, targetPos) > UnitSpeed * Time.deltaTime)
            {
                movingObj.Translate(dir.normalized * UnitSpeed * Time.deltaTime);

                yield return null;
            }
        }

        movingObj.GetComponent<Unit>().PlayAnimation(Anim.walk, false);
        MoveObjectFinish();
    }
    //移动到目的地后
    void MoveObjectFinish()
    {
        GameManager.instance.gamePaused = false;

        //设置节点上的物体，设置英雄所在位置、节点
        BattleManager.instance.LinkNodeWithUnit(movingObj.gameObject, path[path.Count - 1]);

        moving = false;
    }

    float GetHorizontalDistance(Vector3 _p1, Vector3 _p2)
    {
        _p2.z = _p1.z;
        return Vector3.Distance(_p1, _p2);
    }
}
