using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : NodeObject
{
    public Vector2Int interactPoint;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + new Vector3(interactPoint.x, 0, interactPoint.y),
                            Vector3.one * 2);
    }
}
