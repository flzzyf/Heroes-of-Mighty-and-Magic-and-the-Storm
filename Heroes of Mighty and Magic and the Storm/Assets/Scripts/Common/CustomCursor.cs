using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CursorGO
{
    public string cursorName;
    public GameObject cursorGO;
}

public class CustomCursor : Singleton<CustomCursor>
{
    public Vector2 offset;

    public CursorGO[] cursors;

    Dictionary<string, GameObject> cursorDic = new Dictionary<string, GameObject>();

    GameObject currentCursor;

    private void Start()
    {
        foreach (CursorGO item in cursors)
        {
            cursorDic.Add(item.cursorName, item.cursorGO);
        }
    }

    void FixedUpdate()
    {
        transform.position = (Vector2)Input.mousePosition + offset;
    }

    public void ChangeCursor(string _name = "Default")
    {
        for (int i = 0; i < cursors.Length; i++)
        {
            cursors[i].cursorGO.SetActive(false);
        }

        currentCursor = cursorDic[_name];

        cursorDic[_name].SetActive(true);
    }

    public void ChangeCursorAngle(float _angle)
    {
        currentCursor.transform.rotation = Quaternion.AngleAxis(_angle, Vector3.forward);

    }
}
