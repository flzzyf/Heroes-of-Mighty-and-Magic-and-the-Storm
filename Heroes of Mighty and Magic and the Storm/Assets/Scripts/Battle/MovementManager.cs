using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    public float UnitSpeed = 14;

    public static bool moving;

    //按照路径移动物体
    public void MoveObjectAlongPath(Transform _obj, List<GameObject> _path)
    {
        GameManager.instance.gamePaused = true;

        moving = true;

        StartCoroutine(IEMoveObject(_obj, _path));
    }

    IEnumerator IEMoveObject(Transform _obj, List<GameObject> _path)
    {
        for (int i = 1; i < _path.Count; i++)
        {
            Vector3 targetPos = _path[i].transform.position;

            Vector3 dir = targetPos - _obj.position;
            dir.z = 0;
            while (GetHorizontalDistance(_obj.position, targetPos) > UnitSpeed * Time.deltaTime)
            {
                _obj.Translate(dir.normalized * UnitSpeed * Time.deltaTime);

                yield return null;
            }
        }

        MoveObjectFinish(_path[_path.Count - 1]);
    }
    //移动到目的地后
    void MoveObjectFinish(GameObject _endNode)
    {
        GameManager.instance.gamePaused = false;

        moving = false;

        //设置节点上的物体，设置英雄所在位置、节点
        BattleManager.instance.LinkNodeWithUnit(BattleManager.currentActionUnit, _endNode);
    }

    float GetHorizontalDistance(Vector3 _p1, Vector3 _p2)
    {
        _p2.z = _p1.z;
        return Vector3.Distance(_p1, _p2);
    }
}
