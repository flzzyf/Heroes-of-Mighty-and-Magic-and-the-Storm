using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtil
{
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
            // EditorGUILayout.PropertyField(_list.FindPropertyRelative("Array.size"));
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

        if (_list.isArray && _list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton))
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
