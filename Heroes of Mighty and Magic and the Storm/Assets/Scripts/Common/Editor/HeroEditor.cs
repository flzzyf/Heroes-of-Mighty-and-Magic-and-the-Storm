using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Hero))]
public class HeroEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorUtil.ShowList(serializedObject.FindProperty("magics"));

        serializedObject.ApplyModifiedProperties();
    }
}
