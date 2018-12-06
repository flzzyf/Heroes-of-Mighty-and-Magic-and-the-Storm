using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HeroTrait))]
public class HeroTraitEditor : Editor
{
	public override void OnInspectorGUI()
	{
		HeroTrait main = (HeroTrait)target;

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

		//描述文本
		EditorGUILayout.LabelField("描述", LocalizationMgr.instance.GetText(main.name + "_Info"));
		if (GUILayout.Button("编辑"))
		{
			LocalizationWindow.ShowWindow(main.name + "_Info");
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));

		serializedObject.ApplyModifiedProperties();
	}
}
