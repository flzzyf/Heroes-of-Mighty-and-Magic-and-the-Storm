using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public Transform cam;

    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;

    public enum Axis { xy, xz };
    public Axis axis;

    void Update()
    {
        //鼠标在窗口内，否则结束
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return;
        }

        Vector3 dir = GetMovingDir();

        //镜头边缘移动限制
        if (cam.position.x < 0 || cam.position.x > panLimit.x)
            dir.x = 0;

        if (axis == Axis.xy)
        {
            if ((cam.position.y < 0 && dir.y < 0) ||
             (cam.position.y > panLimit.y && dir.y > 0))
                dir.y = 0;
        }
        else
        {
            if ((cam.position.z < 0 && dir.y < 0) ||
                    (cam.position.z > panLimit.y && dir.y > 0))
                dir.y = 0;
        }

        //如果是xz轴移动
        if (axis == Axis.xz)
        {
            dir.z = dir.y;
            dir.y = 0;
        }

        cam.Translate(dir * panSpeed * Time.deltaTime, Space.World);
    }

    //获取镜头移动方向
    Vector2 GetMovingDir()
    {
        Vector2 pos = Vector2.zero;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y = 1;
        }
        else if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.y = -1;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x = -1;
        }
        else if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x = 1;
        }

        return pos;
    }

    void OnDrawGizmos()
    {
        Vector3 mapBorder = new Vector3(panLimit.x, 0, panLimit.y);
        Gizmos.DrawWireCube(mapBorder / 2, mapBorder);
    }
}
