using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateWithMouse : MonoBehaviour
{
    public Vector2 cameraAngleLimit = new Vector2(2, 3);
    public float sensitivity = 1;

    private void Start()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        enabled = false;
#endif
    }

    void Update ()
    {
        Vector2 mousePoint = Input.mousePosition;
        float mouseRateX = (mousePoint.x - Screen.width / 2) / Screen.width / 2 * sensitivity;
        float mouseRateY = (mousePoint.y - Screen.height / 2) / Screen.height / 2 * sensitivity;

        Vector3 angle = new Vector3(-mouseRateY * cameraAngleLimit.x, mouseRateX * cameraAngleLimit.y, 0);
        transform.rotation = Quaternion.Euler(angle);
    }
}
