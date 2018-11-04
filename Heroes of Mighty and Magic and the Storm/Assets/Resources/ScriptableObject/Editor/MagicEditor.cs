﻿using System.Collections;
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

        //魔法描述文本
        EditorGUILayout.LabelField("描述", LocalizationMgr.instance.GetText(main.name + "_Info"));
        if (GUILayout.Button("编辑"))
        {
            LocalizationWindow.ShowWindow(main.name + "_Info");
        }

        //魔法效果文本
        EditorGUILayout.LabelField("效果", LocalizationMgr.instance.GetText(main.name + "_Effect"));
        if (GUILayout.Button("编辑"))
        {
            LocalizationWindow.ShowWindow(main.name + "_Effect");
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("school"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("level"));
        EditorUtil.ShowList(serializedObject.FindProperty("mana"));
        EditorUtil.ShowList(serializedObject.FindProperty("targetType"));
        //if(main.targetType[0] == MagicTargetType.Unit)
        EditorUtil.ShowList(serializedObject.FindProperty("targetFliter"));
        EditorUtil.ShowList(serializedObject.FindProperty("effects"));


        serializedObject.ApplyModifiedProperties();
    }
}
