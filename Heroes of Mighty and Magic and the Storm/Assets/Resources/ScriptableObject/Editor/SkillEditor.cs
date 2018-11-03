using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Skill main = (Skill)target;

        //显示图标
        if (main.icon != null)
        {
            Texture texture = main.icon.texture;
            GUILayout.Box(texture, EditorStyles.objectFieldThumb,
            GUILayout.Width(100f), GUILayout.Height(100f));
        }

        //名称 
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", LocalizationMgr.instance.GetText(main.name));
        if (GUILayout.Button("编辑名称"))
        {
            LocalizationWindow.ShowWindow(main.name);
        }
        EditorGUILayout.EndHorizontal();

        //魔法描述文本
        EditorGUILayout.LabelField("描述", LocalizationMgr.instance.GetText(main.name + "_Info"));
        if (GUILayout.Button("编辑"))
        {
            LocalizationWindow.ShowWindow(main.name + "_Info");
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorUtil.ShowList(serializedObject.FindProperty("amounts"));

        serializedObject.ApplyModifiedProperties();
    }

}
