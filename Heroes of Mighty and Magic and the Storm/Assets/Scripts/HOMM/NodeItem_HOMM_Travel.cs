using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeItem_HOMM_Travel : NodeItem
{
    public GameObject arrow;
    public GameObject goal;

    public bool isGoal;

    public bool reachable;

    public override void UpdateStatus()
    {
        if (isGoal)
        {
            goal.SetActive(true);
            return;
        }

        if(isPath)
        {
            arrow.SetActive(true);

            if (reachable)
            {
                arrow.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            }
            else
            {
                arrow.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }

            return;
        }

        goal.SetActive(false);
        arrow.SetActive(false);
    }

    public void ArrowFaceTarget(GameObject _go)
    {
        LookAtTarget(arrow.transform, _go.transform);
    }

    void LookAtTarget(Transform _origin, Transform _target)
    {
        Vector3 dir = _target.position - _origin.position;
        dir.Normalize();

        LookAtTarget(_origin, dir);
    }

    void LookAtTarget(Transform _origin, Vector3 _dir)
    {
        Quaternion lookRotation = Quaternion.LookRotation(_dir);
        _origin.rotation = lookRotation;
    }
}
