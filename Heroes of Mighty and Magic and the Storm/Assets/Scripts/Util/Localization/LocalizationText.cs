using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    public string key;

    void Awake()
    {
        LocalizationMgr.instance.localizationTexts.Add(this);
        Init();
    }

    public void Init()
    {
        GetComponent<Text>().text = LocalizationMgr.instance.GetText(key);
        GetComponent<Text>().font = LocalizationMgr.instance.font;
    }
}
