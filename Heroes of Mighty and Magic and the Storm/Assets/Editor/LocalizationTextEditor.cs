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
    }
}
