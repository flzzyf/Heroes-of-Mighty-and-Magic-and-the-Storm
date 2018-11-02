using UnityEngine;
using UnityEditor;

public class LocalizationWindow : EditorWindow
{
    public static string key;
    public static string theName = "";

    const int w = 350, h = 200;

    [MenuItem("Zyf Tools/文本编辑器")]
    public static void ShowWindow()
    {
        ShowWindow("");
        theName = "";
    }

    public static void ShowWindow(string _key)
    {
        key = _key;

        Rect rect = new Rect(Screen.width - w / 4, Screen.height / 4, w, h);

        LocalizationWindow window = GetWindow<LocalizationWindow>("wtf");
        window.position = rect;
    }

    void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        key = EditorGUILayout.TextField("Key", key);
        if (EditorGUI.EndChangeCheck())
        {
            if(LocalizationMgr.instance.ContainKey(key))
                theName = LocalizationMgr.instance.GetText(key);
        }

        EditorGUILayout.LabelField("Text");
        theName = EditorGUILayout.TextArea(theName, GUILayout.Height(80));

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
