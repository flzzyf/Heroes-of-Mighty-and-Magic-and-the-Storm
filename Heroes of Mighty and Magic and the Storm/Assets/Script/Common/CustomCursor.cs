using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour {

    public Vector2 offset;

	void FixedUpdate () 
    {
        transform.position = (Vector2)Input.mousePosition + offset;
	}
}
