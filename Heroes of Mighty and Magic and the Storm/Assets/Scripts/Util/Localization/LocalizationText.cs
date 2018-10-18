using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    public string key;

    public string[] args;

    void Awake()
    {
        LocalizationMgr.instance.localizationTexts.Add(this);
    }

    public void ChangeToLanguage(Language _language)
    {
        LocalizationMgr.instance.LoadLanguage(_language);

        Init();
    }

    public void Init()
    {
        GetComponent<Text>().text = LocalizationMgr.instance.GetText(key);
        GetComponent<Text>().font = LocalizationMgr.instance.font;
    }
}
