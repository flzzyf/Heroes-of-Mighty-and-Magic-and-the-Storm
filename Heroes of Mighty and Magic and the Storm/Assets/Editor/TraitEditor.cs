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

        //检测名称更改
        EditorGUI.BeginChangeCheck();
        string name = EditorGUILayout.TextField("Name", LocalizationMgr.instance.GetText(trait.name));
        if (EditorGUI.EndChangeCheck())
        {
            LocalizationMgr.instance.SetText(trait.name, name);
        }
        EditorGUILayout.LabelField("名称修改后自动保存");
    }
}
