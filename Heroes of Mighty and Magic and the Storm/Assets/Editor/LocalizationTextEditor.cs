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
        base.OnInspectorGUI();

        GUILayout.Label("切换语言：");
        foreach (var item in LocalizationMgr.instance.languageDic)
        {
            if (GUILayout.Button(item.Value.name))
            {
                ChangeLanguage(item.Key);
            }
        }
    }

    public void ChangeLanguage(LanguageName _languageName)
    {
        LocalizationMgr.instance.ChangeToLanguage(_languageName);
    }

}
