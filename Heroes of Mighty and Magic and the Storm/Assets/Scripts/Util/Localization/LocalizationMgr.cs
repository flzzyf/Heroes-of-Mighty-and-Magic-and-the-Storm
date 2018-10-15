using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SubjectNerd.Utilities;

public enum Language { Chinese_Simplified, Chinese_Traditional, English }

[System.Serializable]
public class LanguageFont
{
    public Language language;
    public Font font;
}

public class LocalizationMgr : Singleton<LocalizationMgr>
{
    public Language language;
    [HideInInspector]
    public Font font;

    [Reorderable]
    public List<LanguageFont> languageFont;

    private Dictionary<string, string> dic;
    private Dictionary<Language, Font> fontDic;

    [HideInInspector]
    public List<LocalizationText> localizationTexts;

    void Start()
    {
        fontDic = new Dictionary<Language, Font>();
        foreach (var item in languageFont)
        {
            fontDic[item.language] = item.font;
        }

        ChangeToLanguage(language);
    }

    public void ChangeToLanguage(Language _language)
    {
        language = _language;
        font = fontDic[language];
        LoadLanguage(_language);
        InitAllLocalizationTexts(_language);
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
    void LoadLanguage(Language _language)
    {
        dic = new Dictionary<string, string>();
        TextAsset ta = Resources.Load<TextAsset>(_language.ToString());
        string text = ta.text;

        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (line == null || line == "")
                continue;

            string[] s = line.Split('=');
            dic.Add(s[0], s[1]);
        }
    }
    //从字典读取相应文本
    public string GetText(string _key)
    {
        if (dic.ContainsKey(_key))
            return System.Text.RegularExpressions.Regex.Unescape(dic[_key]);

        return _key.ToString();
    }

    public void SetText(Text _text, string _key)
    {
        _text.text = GetText(_key);
        _text.font = font;
    }
}
