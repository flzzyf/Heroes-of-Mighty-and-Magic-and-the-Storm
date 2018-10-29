using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager_Battle))]
public class MapManager_BattleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("重置并生成战场"))
        {
            MapManager_Battle origin = (MapManager_Battle)target;
            origin.ClearMap();
            origin.GenerateMap();
        }
    }
}
