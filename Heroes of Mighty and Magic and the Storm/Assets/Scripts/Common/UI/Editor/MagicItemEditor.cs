using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MagicItem))]
public class MagicItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MagicItem main = (MagicItem)target;

        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("等级文本", LocalizationMgr.instance.GetText(main.key_level));
        if (GUILayout.Button("编辑文本"))
        {
            LocalizationWindow.ShowWindow(main.key_level);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("蓝耗文本", LocalizationMgr.instance.GetText(main.key_mana));
        if (GUILayout.Button("编辑文本"))
        {
            LocalizationWindow.ShowWindow(main.key_mana);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("初始化"))
        {
            main.Init();
        }

        serializedObject.ApplyModifiedProperties();

    }
}
