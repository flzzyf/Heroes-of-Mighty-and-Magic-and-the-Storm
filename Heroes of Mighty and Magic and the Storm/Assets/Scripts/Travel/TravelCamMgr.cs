using System.Collections.Generic;
using UnityEngine;

public class TravelCamMgr : Singleton<TravelCamMgr>
{
    public GameObject cam;

    //移动镜头到目的地
    public void MoveCamera(Vector3 _pos)
    {
        _pos.y = cam.transform.position.y;
        cam.transform.position = _pos;
    }
}
