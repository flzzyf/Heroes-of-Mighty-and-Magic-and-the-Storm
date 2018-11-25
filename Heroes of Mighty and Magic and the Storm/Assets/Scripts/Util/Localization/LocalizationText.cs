using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    public string key;
    public string[] args;
    Text text;

    void OnEnable()
    {
        text = GetComponent<Text>();
        LocalizationMgr.instance.localizationTexts.Add(this);

        Init();
    }

    public void ChangeToLanguage(Language _language)
    {
        LocalizationMgr.instance.LoadLanguage(_language);

        Init();
    }

    public void Init()
    {
        if (key == "")
            return;

        text.font = LocalizationMgr.instance.font;

        if (args.Length == 0)
        {
            text.text = LocalizationMgr.instance.GetText(key);
        }
        else
        {
            text.text = string.Format(LocalizationMgr.instance.GetText(key), args);
        }
    }

    public void SetText(string _key, params string[] _text)
    {
        key = _key;
        args = _text;

        Init();
    }

    public void SetText(string _key)
    {
        key = _key;

        Init();
    }

    public void ClearText()
    {
        text.text = "";
    }
}
