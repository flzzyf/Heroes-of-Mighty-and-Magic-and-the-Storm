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

        if (trait.currentLanguage != LocalizationMgr.instance.language)
        {
            trait.currentLanguage = LocalizationMgr.instance.language;
        }
        //检测名称更改
        EditorGUI.BeginChangeCheck();
        string name = EditorGUILayout.TextField("Name", LocalizationMgr.instance.GetText(trait.name));
        if (EditorGUI.EndChangeCheck())
        {
            LocalizationMgr.instance.SetText(trait.name, name);
        }
        EditorGUILayout.LabelField("名称修改后自动保存");

        base.OnInspectorGUI();
    }
}
