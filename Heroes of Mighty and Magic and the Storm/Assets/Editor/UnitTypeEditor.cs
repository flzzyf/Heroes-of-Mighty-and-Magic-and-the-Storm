using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitType))]
public class UnitTypeEditor : Editor
{
    string[] tab_main = { "level", "att", "def", "damage", "hp",
        "speed", "growth", "AIValue", "cost", "traits", "attackType"};

    string[] tab_other = { "icon", "size", "armorType", "race", "animControl", };
    string[] tab_sound = { "sound_attack", "sound_walk", "sound_hit", "sound_death" };

    public override void OnInspectorGUI()
    {
        UnitType type = (UnitType)target;

        type.tab = GUILayout.Toolbar(type.tab, new string[] { "Main", "Other", "Sound" });

        if (type.tab == 0)
        {
            if (LocalizationMgr.instance.textDic == null)
            {
                LocalizationMgr.instance.LoadLanguage(LocalizationMgr.instance.language);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", type.unitName);
            if (GUILayout.Button("编辑名称"))
            {
                LocalizationWindow.ShowWindow(type.name);
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < tab_main.Length; i++)
            {
                SerializedProperty property = serializedObject.FindProperty(tab_main[i]);
                if (property.isArray)
                {
                    EditorUtil.ShowList(property);
                }
                else
                {
                    EditorGUILayout.PropertyField(property);
                }

                //远程攻击特殊选项
                if (tab_main[i] == "attackType")
                {
                    if (type.attackType == AttackType.range)
                    {
                        EditorGUI.indentLevel += 1;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ammo"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("missile"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("launchPos"));
                        EditorGUI.indentLevel -= 1;
                    }
                }
            }
        }
        else if (type.tab == 1)
        {
            for (int i = 0; i < tab_other.Length; i++)
            {
                SerializedProperty property = serializedObject.FindProperty(tab_other[i]);

                if (tab_other[i] == "icon")
                {
                    if (type.icon != null)
                    {
                        Texture texture = type.icon.texture;
                        GUILayout.Box(texture, EditorStyles.objectFieldThumb,
                        GUILayout.Width(100f), GUILayout.Height(100f));
                    }
                }

                if (property.isArray)
                {
                    EditorUtil.ShowList(property);
                }
                else
                {
                    EditorGUILayout.PropertyField(property);
                }
            }
        }
        else if (type.tab == 2)
        {
            for (int i = 0; i < tab_sound.Length; i++)
            {
                SerializedProperty property = serializedObject.FindProperty(tab_sound[i]);
                if (property.isArray)
                {
                    EditorUtil.ShowList(property);
                }
                else if (property.type == typeof(FixedSound).Name)
                {
                    EditorGUILayout.PropertyField(property);
                    if (property.isExpanded)
                    {
                        EditorGUI.indentLevel += 1;
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("clip"), GUIContent.none);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("skipDuration"));
                        EditorGUI.indentLevel -= 1;
                    }
                }
                else
                    EditorGUILayout.PropertyField(property);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
