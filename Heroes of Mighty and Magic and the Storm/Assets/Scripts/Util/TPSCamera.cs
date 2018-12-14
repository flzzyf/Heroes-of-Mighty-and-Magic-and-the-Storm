using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamera : MonoBehaviour 
{
	public Transform cam;

	public float panSpeed = 20f;
	public float panBorderThickness = 10f;
	public Vector2 panLimit;

	public enum Axis { xy, xz};
	public Axis axis;

	void Update()
	{
		Vector3 pos = Vector3.zero;

		if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
		{
			if (axis == Axis.xy)
				pos.y = 1;
			else
				pos.z = 1;
		}
		if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
		{
			if (axis == Axis.xy)
				pos.y = -1;
			else
				pos.z = -1;
		}
		if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
		{
			pos.x = -1;
		}
		else if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
		{
			pos.x = 1;
		}

		//边缘移动限制
		//pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
		//pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);

		cam.Translate(pos * panSpeed * Time.deltaTime, Space.World);

	}
}
