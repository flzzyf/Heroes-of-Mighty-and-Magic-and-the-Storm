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
        base.OnInspectorGUI();

        mgr.languageDic = new Dictionary<LanguageName, Language>();
        //初始化DIC

        //为每个预设语言创建UI
        for (int i = 0; i < System.Enum.GetValues(typeof(LanguageName)).Length; i++)
        {
            string name = System.Enum.GetName(typeof(LanguageName), i);
            LanguageName languageName = (LanguageName)i;
            //Language language = mgr.languageDic[languageName];

            GUILayout.Label(name);

            mgr.languageDic[languageName].name = GUILayout.TextArea("");

            if (GUILayout.Button(languageName.ToString()))
            {
                mgr.ChangeToLanguage(languageName);
            }

            GUILayout.Space(20);
        }
    }
}
