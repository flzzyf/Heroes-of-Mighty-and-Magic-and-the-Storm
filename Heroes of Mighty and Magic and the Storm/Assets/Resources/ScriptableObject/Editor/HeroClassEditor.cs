using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HeroClass))]
public class HeroClassEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HeroClass main = (HeroClass)target;

        //名称 
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", LocalizationMgr.instance.GetText(main.name));
        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(main.name);
        }
        EditorGUILayout.EndHorizontal();

        //描述文本
        EditorGUILayout.LabelField("描述", LocalizationMgr.instance.GetText(main.name + "_Info"));
        if (GUILayout.Button("编辑"))
        {
            LocalizationWindow.ShowWindow(main.name + "_Info");
        }

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }
}
