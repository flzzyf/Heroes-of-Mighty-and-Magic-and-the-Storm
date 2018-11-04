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

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", trait.traitName);
        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(trait.name);
        }
        EditorGUILayout.EndHorizontal();
    }
}
