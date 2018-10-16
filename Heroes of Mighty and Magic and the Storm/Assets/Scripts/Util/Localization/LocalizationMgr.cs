using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LanguageName { Chinese_Simplified, Chinese_Traditional, English }

[System.Serializable]
public class Language
{
    public string name;
    public Font font;
}

public class LocalizationMgr : Singleton<LocalizationMgr>
{
    Language language;

    LanguageName languageName;

    public Font[] fonts;

    public int[] a;

    //public List<Language> languageList;

    public Dictionary<LanguageName, Language> languageDic;

    private Dictionary<string, string> textDic;

    [HideInInspector]
    public List<LocalizationText> localizationTexts;

    public Font font { get { return language.font; } }

    void Awake()
    {
        Init();
        ChangeToLanguage(languageName);
    }

    public void Init()
    {
        languageDic = new Dictionary<LanguageName, Language>();
        for (int i = 0; i < System.Enum.GetValues(typeof(LanguageName)).Length; i++)
        {
            LanguageName languageName = (LanguageName)i;

            languageDic[languageName] = new Language();
        }
    }

    public void ChangeToLanguage(LanguageName _language)
    {
        language = languageDic[_language];
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
    void LoadLanguage(Language _language)
    {
        textDic = new Dictionary<string, string>();
        TextAsset ta = Resources.Load<TextAsset>(_language.ToString());
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
