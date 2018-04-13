using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementManager : MonoBehaviour
{
    Map_HOMMS map;

    GameObject movingUnit;

    List<Node> path;

    int currentWayPointIndex = 0;
    Vector3 targetWayPoint;

    public float unitSpeed = 8;

    public event EventHandler MoveComplete;


    private void Start()
    {
        map = GetComponent<Map_HOMMS>();
    }

    public void MoveUnit(GameObject _unit, List<Node> _path)
    {
        movingUnit = _unit;
        path = _path;

        movingUnit.GetComponent<Unit>().PlayAnimation("move", 1);

        //将先前的格子设为无人
        movingUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node.walkable = true;
        movingUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().unit = null;

        GetNextWayPoint();
        GameMaster.instance.Pause();

        StartCoroutine(Moving());

    }

    IEnumerator Moving()
    {
        while (movingUnit != null)
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
        movingUnit.GetComponent<Unit>().PlayAnimation("move", 0);

        //进入新格子
        Node currentNode = path[currentWayPointIndex];
        movingUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node = currentNode;
        movingUnit.GetComponent<Unit>().nodeUnit = map.GetNodeUnit(currentNode);
        map.GetNodeUnit(currentNode).GetComponent<NodeUnit>().unit = movingUnit;
        currentNode.walkable = false;

        //重置移动单位信息
        movingUnit = null;
        currentWayPointIndex = 0;

        path.Clear();
        map.path.Clear();

        GameMaster.instance.Unpause();

        MoveComplete(this, EventArgs.Empty);
    }

    //获取下个路径点
    void GetNextWayPoint()
    {
        targetWayPoint = map.GetNodeUnit(path[currentWayPointIndex]).transform.position;
    }
}
