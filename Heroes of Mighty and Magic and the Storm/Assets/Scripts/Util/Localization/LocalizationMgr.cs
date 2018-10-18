using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum Language { Chinese_Simplified, Chinese_Traditional, English }

public class LocalizationMgr : Singleton<LocalizationMgr>
{
    public Language language;

    public string[] languageNames = new string[System.Enum.GetValues(typeof(Language)).Length];
    public Font[] fonts = new Font[Enum.GetValues(typeof(Language)).Length];

    Dictionary<string, string> textDic;
    //所有本地化文本组件
    public List<LocalizationText> localizationTexts;

    public Font font { get { return fonts[(int)language]; } }

    void Start()
    {
        ChangeToLanguage(language);
    }

    public void ChangeToLanguage(Language _language)
    {
        LoadLanguage(language);
        InitAllLocalizationTexts(language);
    }
    //初始化所有本地化文本为相应文本
    void InitAllLocalizationTexts(Language _language)
    {
        foreach (LocalizationText text in localizationTexts)
        {
            text.Init();
        }
    }
    //加载语音文件，将内容放入字典
    public void LoadLanguage(Language _language)
    {
        language = _language;

        textDic = new Dictionary<string, string>();
        TextAsset ta = Resources.Load<TextAsset>("Localization/" + _language.ToString());

        string text = ta.text;

        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (line == null || line == "")
                continue;

            string[] s = line.Split('=');
            textDic.Add(s[0], s[1]);
        }
    }
    //从字典读取相应文本
    public string GetText(string _key)
    {
        if (textDic.ContainsKey(_key))
            return System.Text.RegularExpressions.Regex.Unescape(textDic[_key]);

        return _key.ToString();
    }

    public void SetText(Text _text, string _key)
    {
        _text.text = GetText(_key);
    }
}
