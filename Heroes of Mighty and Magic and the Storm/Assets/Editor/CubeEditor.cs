using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Flags]
public enum EditorListOption
{
    None = 0, ListSize = 1, ListLabel = 2, Default = ListSize | ListLabel
}

[CustomEditor(typeof(Cube))]
public class Cubeditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ShowList(serializedObject.FindProperty("a"), EditorListOption.ListLabel);
        ShowList(serializedObject.FindProperty("f"), EditorListOption.ListSize);

        serializedObject.ApplyModifiedProperties();
    }

    static void ShowList(SerializedProperty _list, EditorListOption _option)
    {
        bool showListLabel = (_option & EditorListOption.ListLabel) != 0;
        bool showListSize = (_option & EditorListOption.ListSize) != 0;

        if (showListLabel)
        {
            EditorGUILayout.PropertyField(_list);
            EditorGUI.indentLevel += 1;
        }
        if (!showListLabel || _list.isExpanded)
        {
            if (showListSize)
                EditorGUILayout.PropertyField(_list.FindPropertyRelative("Array.size"));
            for (int i = 0; i < _list.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i));
            }
        }
        if (showListLabel)
            EditorGUI.indentLevel -= 1;
    }
}
