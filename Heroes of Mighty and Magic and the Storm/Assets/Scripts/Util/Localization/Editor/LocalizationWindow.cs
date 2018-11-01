using UnityEngine;
using UnityEditor;

public class LocalizationWindow : EditorWindow
{
    public static string key;
    public static string theName;

    const int w = 300, h = 100;

    public static void ShowWindow(string _key)
    {
        key = _key;

        Rect rect = new Rect(Screen.width - w / 4, Screen.height / 4, w, h);

        LocalizationWindow window = GetWindow<LocalizationWindow>("wtf");
        window.position = rect;
    }

    void OnEnable()
    {
        theName = LocalizationMgr.instance.GetText(key);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Key", key);
        //theName = EditorGUILayout.TextField("Name", theName);
        EditorGUILayout.LabelField("Text");
        theName = EditorGUILayout.TextArea(theName);

        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("修改"))
        {
            LocalizationMgr.instance.SetText(key, theName);
            this.Close();
        }
        if (GUILayout.Button("关闭"))
        {
            this.Close();
        }
        GUILayout.EndHorizontal();
    }
}
