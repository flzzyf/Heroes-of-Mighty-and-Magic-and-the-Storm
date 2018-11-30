using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sound))]
[CanEditMultipleObjects]
public class SoundEditor : Editor
{
	string[] attributes = { "clips", "volume", "pitch", "loop", "group", "startingTime" };

	public override void OnInspectorGUI()
	{
		Sound origin = (Sound)target;

		for (int i = 0; i < attributes.Length; i++)
		{
			SerializedProperty property = serializedObject.FindProperty(attributes[i]);
			if (property.isArray)
			{
				EditorUtil.ShowList(property);
			}
			else
			{
				EditorGUILayout.PropertyField(property);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
