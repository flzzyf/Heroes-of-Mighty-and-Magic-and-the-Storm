using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocalizationText))]
public class LocalizationTextEditor : Editor
{
    LocalizationText text;

    void Awake()
    {
        text = (LocalizationText)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));

        SerializedProperty list = serializedObject.FindProperty("args");
        ShowList(list);

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

    public void ShowList(SerializedProperty _list)
    {
        //显示数列标签
        EditorGUILayout.PropertyField(_list);
        if (_list.isExpanded)
        {
            //显示数列大小
            EditorGUILayout.PropertyField(_list.FindPropertyRelative("Array.size"));
            //显示数列子元素
            for (int i = 0; i < _list.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i));
            }
        }
    }

}
