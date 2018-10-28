using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CursorGO
{
    public string name;
    public GameObject cursorGO;
}

public class CursorManager : Singleton<CursorManager>
{
    public Vector2 offset;

    public Transform cursorParent;
    public CursorGO[] cursors;

    GameObject currentCursor;
    string currentCursorName;

    void Start()
    {
        Cursor.visible = false;

        ChangeCursor();
    }

    void Update()
    {
        cursorParent.position = (Vector2)Input.mousePosition + offset;
    }

    public void ChangeCursor(string _name = "default")
    {
        if (currentCursorName != _name)
            currentCursorName = _name;

        for (int i = 0; i < cursors.Length; i++)
        {
            cursors[i].cursorGO.SetActive(false);
        }

        for (int i = 0; i < cursors.Length; i++)
        {
            if (_name == cursors[i].name)
            {
                cursors[i].cursorGO.SetActive(true);
                return;
            }
        }

        print("未能找到指针");
    }

    //改变指针角度
    public void ChangeCursorAngle(float _angle = 0)
    {
        cursorParent.rotation = Quaternion.AngleAxis(_angle, Vector3.forward);
    }
}
