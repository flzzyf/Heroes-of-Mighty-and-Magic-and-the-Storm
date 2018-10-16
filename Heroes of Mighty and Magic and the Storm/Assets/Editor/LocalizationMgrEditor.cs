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
        // if (mgr.languageDic == null)
        // {
        //     Debug.Log("qwe");
        //     mgr.Init();
        // }

    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();

        ShowList(serializedObject.FindProperty("a"), true, false);
        ShowList(serializedObject.FindProperty("fonts"));

        //为每个预设语言创建UI
        for (int i = 0; i < System.Enum.GetValues(typeof(LanguageName)).Length; i++)
        {
            string name = System.Enum.GetName(typeof(LanguageName), i);
            LanguageName languageName = (LanguageName)i;
            //Language language = mgr.languageDic[languageName];

            GUILayout.Label(name);

            //mgr.languageDic[languageName].name = GUILayout.TextArea(mgr.languageDic[languageName].name);


            //切换语言的按钮
            if (GUILayout.Button(languageName.ToString()))
            {
                mgr.ChangeToLanguage(languageName);
            }

            GUILayout.Space(20);
        }

        serializedObject.ApplyModifiedProperties();
    }

    static void ShowList(SerializedProperty _list, bool _showListSize = true, bool _showListLabel = true)
    {
        if (_showListLabel)
        {
            EditorGUILayout.PropertyField(_list);
            EditorGUI.indentLevel += 1;
        }
        if (!_showListLabel || _list.isExpanded)
        {
            if (_showListSize)
                EditorGUILayout.PropertyField(_list.FindPropertyRelative("Array.size"));
            for (int i = 0; i < _list.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i));
            }
        }
        if (_showListLabel)
            EditorGUI.indentLevel -= 1;
    }
}
