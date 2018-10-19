using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    public string key;
    public string[] args;

    void OnEnable()
    {
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

        GetComponent<Text>().font = LocalizationMgr.instance.font;

        if (args.Length == 0)
        {
            GetComponent<Text>().text = LocalizationMgr.instance.GetText(key);
        }
        else
        {
            GetComponent<Text>().text = string.Format(LocalizationMgr.instance.GetText(key), args);
        }
    }

    public void SetText(string _key, params string[] _text)
    {
        key = _key;
        args = _text;

        Init();
    }
}
