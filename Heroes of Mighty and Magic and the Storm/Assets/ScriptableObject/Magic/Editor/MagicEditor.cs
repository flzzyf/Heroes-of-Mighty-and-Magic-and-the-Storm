using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Magic))]
public class MagicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Magic main = (Magic)target;

        //显示图标
        if (main.icon != null)
        {
            Texture texture = main.icon.texture;
            GUILayout.Box(texture, EditorStyles.objectFieldThumb,
            GUILayout.Width(100f), GUILayout.Height(100f));
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", LocalizationMgr.instance.GetText(main.name));
        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(main.name);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("school"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("level"));
        EditorUtil.ShowList(serializedObject.FindProperty("mana"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effect"));

        serializedObject.ApplyModifiedProperties();
    }
}
