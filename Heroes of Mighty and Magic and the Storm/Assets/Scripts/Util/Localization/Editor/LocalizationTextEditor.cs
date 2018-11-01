using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocalizationText))]
public class LocalizationTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LocalizationText text = (LocalizationText)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));

        EditorGUILayout.BeginHorizontal();
        if(text.key != null)
            EditorGUILayout.LabelField("Name", LocalizationMgr.instance.GetText(text.key));
        else
            EditorGUILayout.LabelField("Name", "");

        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(text.key);
        }
        EditorGUILayout.EndHorizontal();

        SerializedProperty list = serializedObject.FindProperty("args");
        EditorUtil.ShowList(list);

        for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        {
            GUILayout.Space(20);

            Language language = (Language)i;
            //切换语言的按钮
            if (GUILayout.Button("切换到" + LocalizationMgr.instance.languageNames[i]))
            {
                text.ChangeToLanguage(language);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
