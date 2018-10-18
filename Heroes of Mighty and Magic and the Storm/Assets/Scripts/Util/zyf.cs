using UnityEngine;
using UnityEditor;

public class zyf
{
    public static Vector2 GetWorldScreenSize()
    {
        float leftBorder;
        float rightBorder;
        float topBorder;
        float downBorder;
        //the up right corner
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Mathf.Abs(Camera.main.transform.position.z)));

        leftBorder = Camera.main.transform.position.x - (cornerPos.x - Camera.main.transform.position.x);
        rightBorder = cornerPos.x;
        topBorder = cornerPos.y;
        downBorder = Camera.main.transform.position.y - (cornerPos.y - Camera.main.transform.position.y);

        float width = rightBorder - leftBorder;
        float height = topBorder - downBorder;

        return new Vector2(width, height);
    }

    public static bool IfItWins(int _a)
    {
        return Random.Range(0, _a) == 1;
    }

    public static Vector3 GetMouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    //自定义UI
    private static GUIContent
            moveButtonContent = new GUIContent("\u21b4", "move down"),
            duplicateButtonContent = new GUIContent("+", "duplicate"),
            deleteButtonContent = new GUIContent("-", "delete"),
            addButtonContent = new GUIContent("+", "add");
    private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    public static void ShowList(SerializedProperty _list)
    {
        //显示数列标签
        EditorGUILayout.PropertyField(_list);
        if (_list.isExpanded)
        {
            EditorGUI.indentLevel += 1;
            //显示数列大小
            EditorGUILayout.PropertyField(_list.FindPropertyRelative("Array.size"));
            //显示数列子元素
            for (int i = 0; i < _list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i), GUIContent.none);

                ShowButtons(_list, i);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel -= 1;
        }

        if (_list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton))
        {
            _list.arraySize += 1;
        }

        GUILayout.Space(10);
    }

    private static void ShowButtons(SerializedProperty _list, int _index)
    {
        if (_index != _list.arraySize - 1)
        {
            if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
                _list.MoveArrayElement(_index, _index + 1);
        }
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
            _list.InsertArrayElementAtIndex(_index);
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            int oldSize = _list.arraySize;
            _list.DeleteArrayElementAtIndex(_index);
            if (_list.arraySize == oldSize)
            {
                _list.DeleteArrayElementAtIndex(_index);
            }
        }
    }
}