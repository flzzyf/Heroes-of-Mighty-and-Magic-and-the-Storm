using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisZControl : MonoBehaviour {

	void Update () 
    {
        Vector3 newPos = transform.position;
        newPos.z = transform.position.y;
        transform.position = newPos;
	}
}
