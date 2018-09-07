using UnityEditor;

[CustomEditor(typeof(Button_Special))]
public class Button_SpecialEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Button_Special button = (Button_Special)target;

        base.OnInspectorGUI();
    }
}
