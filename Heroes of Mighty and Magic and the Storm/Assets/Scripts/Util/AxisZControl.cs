using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisZControl : MonoBehaviour
{
    public float offsetZ;

    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.z = transform.position.y + offsetZ;
        transform.position = newPos;
    }
}
