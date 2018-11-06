using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MainMenuButton))]
public class MainMenuButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MainMenuButton main = (MainMenuButton)target;

        base.OnInspectorGUI();

        //Editor修改过的东西要SetDirty，不然不会保存

        if (main.textComponent.text != main.text)
        {
            main.textComponent.text = main.text;
            EditorUtility.SetDirty(main.textComponent);
        }

        if (main.bgImage.sprite != main.bg)
        {
            main.bgImage.sprite = main.bg;
            EditorUtility.SetDirty(main.bgImage);
        }

        if (main.bgOutline.sprite != main.bg)
        {
            main.bgOutline.sprite = main.bg;
            EditorUtility.SetDirty(main.bgOutline);
        }

        if (main.bgOutline.rectTransform.localScale != main.bgImage.rectTransform.localScale)
            main.bgOutline.rectTransform.localScale = main.bgImage.rectTransform.localScale;

        if (main.bgOutline.rectTransform.position != main.bgImage.rectTransform.position)
            main.bgOutline.rectTransform.position = main.bgImage.rectTransform.position;

        EditorUtility.SetDirty(main.bgOutline.rectTransform);

        serializedObject.ApplyModifiedProperties();
    }
}
