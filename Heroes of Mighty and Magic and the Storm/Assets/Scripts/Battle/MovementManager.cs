using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementManager : MonoBehaviour
{
    Map_HOMMS map;

    Unit movingUnit;

    List<AstarNode> path;

    int currentWayPointIndex = 0;
    Vector3 targetWayPoint;

    public float unitSpeed = 8;

    public bool moving;

    private void Start()
    {
        map = GetComponent<Map_HOMMS>();
    }

    public void MoveUnit(GameObject _unit, List<AstarNode> _path)
    {
        movingUnit = _unit.GetComponent<Unit>();
        path = _path;

        movingUnit.PlayAnimation("move", 1);

        //将先前的格子设为无人
        BattleManager.Instance().UnlinkNodeWithUnit(_unit);
        //movingUnit.nodeUnit.GetComponent<NodeUnit>().node.walkable = true;
        //movingUnit.nodeUnit.GetComponent<NodeUnit>().unit = null;

        GetNextWayPoint();
        //暂停

        StartCoroutine(Moving());

    }

    IEnumerator Moving()
    {
        moving = true;
        while (moving)
        {
            //朝下个点方向
            Vector2 dir = targetWayPoint - movingUnit.transform.position;
            //移动
            movingUnit.transform.Translate(dir.normalized * unitSpeed * Time.fixedDeltaTime, Space.World);
            //抵达
            if (dir.magnitude <= unitSpeed * Time.fixedDeltaTime)
            {
                ReachPoint();
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    void ReachPoint()
    {
        if (currentWayPointIndex < path.Count - 1)
        {
            currentWayPointIndex++;
            GetNextWayPoint();

        }
        else
        {
            ReachTarget();
        }

    }

    void ReachTarget()
    {
        moving = false;

        movingUnit.PlayAnimation("move", 0);

        movingUnit.RestoreFacing();

        //进入新格子
        AstarNode currentNode = path[currentWayPointIndex];

        BattleManager.Instance().LinkNodeWithUnit(movingUnit.gameObject, map.GetNodeUnit(currentNode));

        //重置移动单位信息
        movingUnit = null;
        currentWayPointIndex = 0;

        path.Clear();
        map.path.Clear();

        //暂停
    }

    //获取下个路径点
    void GetNextWayPoint()
    {
        targetWayPoint = map.GetNodeUnit(path[currentWayPointIndex]).transform.position;

        movingUnit.FaceTarget(targetWayPoint);

    }
}
