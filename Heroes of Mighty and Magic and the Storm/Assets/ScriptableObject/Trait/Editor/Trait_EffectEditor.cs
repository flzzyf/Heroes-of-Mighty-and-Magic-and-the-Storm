using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Trait_Effect))]
public class Trait_EffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Trait trait = (Trait)target;

        if (LocalizationMgr.instance.textDic == null)
        {
            LocalizationMgr.instance.LoadLanguage(LocalizationMgr.instance.language);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", trait.traitName);
        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(trait.name);
        }
        EditorGUILayout.EndHorizontal(); 

        base.OnInspectorGUI();
    }
}
