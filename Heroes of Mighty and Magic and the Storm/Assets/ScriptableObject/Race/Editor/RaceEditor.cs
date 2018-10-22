using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Race))]
public class RaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Race main = (Race)target;

        //显示图标
        if (main.sprite_bg != null)
        {
            Texture texture = main.sprite_bg.texture;
            GUILayout.Box(texture, EditorStyles.objectFieldThumb,
            GUILayout.Width(100f), GUILayout.Height(100f));
        }

        if (LocalizationMgr.instance.textDic == null)
        {
            LocalizationMgr.instance.LoadLanguage(LocalizationMgr.instance.language);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", LocalizationMgr.instance.GetText(main.name));
        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(main.name);
        }
        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
