using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Trait))]
public class TraitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Trait trait = (Trait)target;

        if (LocalizationMgr.instance.textDic == null)
        {
            LocalizationMgr.instance.LoadLanguage(LocalizationMgr.instance.language);
        }

        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(trait.name);
        }

        EditorGUILayout.LabelField("Name", trait.traitName);
    }
}
