using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocalizationMgr))]
public class LocalizationMgrEditor : Editor
{
    LocalizationMgr mgr;

    void OnEnable()
    {
        mgr = (LocalizationMgr)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty names = serializedObject.FindProperty("languageNames");
        SerializedProperty fonts = serializedObject.FindProperty("fonts");

        GUILayout.BeginHorizontal();
        GUILayout.Label("当前语言：");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("language"), GUIContent.none);
        GUILayout.EndHorizontal();

        //为每个预设语言创建UI
        for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        {
            GUILayout.Space(15);

            string name = System.Enum.GetName(typeof(Language), i);
            Language language = (Language)i;

            GUILayout.Label(name);

            //语言名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("语言名称：");
            EditorGUILayout.PropertyField(names.GetArrayElementAtIndex(i), GUIContent.none);
            GUILayout.EndHorizontal();

            //字体
            GUILayout.BeginHorizontal();
            GUILayout.Label("字体：");
            EditorGUILayout.PropertyField(fonts.GetArrayElementAtIndex(i), GUIContent.none);
            GUILayout.EndHorizontal();

            //切换语言的按钮
            if (GUILayout.Button("切换到" + names.GetArrayElementAtIndex(i).stringValue))
            {
                mgr.ChangeToLanguage(language);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }


}
