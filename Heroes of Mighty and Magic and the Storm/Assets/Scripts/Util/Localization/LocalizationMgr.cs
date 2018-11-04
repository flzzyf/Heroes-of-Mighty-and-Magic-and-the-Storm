using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public enum Language { Chinese_Simplified, Chinese_Traditional, English }

public class LocalizationMgr : Singleton<LocalizationMgr>
{
    public Language language;

    public string[] languageNames = new string[System.Enum.GetValues(typeof(Language)).Length];
    public Font[] fonts = new Font[Enum.GetValues(typeof(Language)).Length];

    public Dictionary<string, string> textDic;
    //所有本地化文本组件
    public List<LocalizationText> localizationTexts;
    public List<LocalizationFont> localizationFonts;

    public Font font { get { return fonts[(int)language]; } }

    void Start()
    {
        ChangeToLanguage(language);
    }

    public void ChangeToLanguage(Language _language)
    {
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

        foreach (LocalizationFont text in localizationFonts)
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
            if (line == null || line == "" || !line.Contains("="))
                continue;

            string[] s = line.Split(new[] { '=' }, 2);
            textDic.Add(s[0], s[1]);
        }
    }

    //新增/修改词条，新增时同时修改所有语言
    public void SetText(string _key, string _value)
    {
        Language currentLang = language;
        for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        {
            Language lang = (Language)i;
            LoadLanguage(lang);
            if (lang == currentLang)
            {
                if (textDic.ContainsKey(_key))
                {
                    Debug.Log("设置文本，Key: " + _key + ", Value: " + _value);

                    textDic[_key] = _value;
                    SaveText();
                }
            }
            if (!textDic.ContainsKey(_key))
            {
                if (lang == currentLang)
                {
                    Debug.Log("加入文本，Key: " + _key + ", Value: " + _value);

                    textDic.Add(_key, _value);
                    AppendText(_key, _value);
                }
                else
                {
                    Debug.Log("加入文本，Key: " + _key + ", Value: " + _key);

                    textDic.Add(_key, _key);
                    AppendText(_key, _key);
                }

                //SaveText();
            }
        }

        LoadLanguage(currentLang);
    }

    void SaveText()
    {
        string s = "";
        foreach (var item in textDic)
        {
            s += item.Key.ToString();
            s += "=";
            s += item.Value.ToString();
            s += "\n";
        }

        Debug.Log(s);

        File.WriteAllText("Assets/Resources/Localization/" + language.ToString() + ".txt", s);
        LoadLanguage(language);
    }

    void AppendText(string _key, string _text)
    {
        string s = _key + "=" + _text + "\n";

        File.AppendAllText("Assets/Resources/Localization/" + language.ToString() + ".txt", s);
    }

    //删除词条，同时删除其他所有语言中的
    public void DeleteKey(string _key)
    {
        Language currentLang = language;
        for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        {
            Language lang = (Language)i;
            if (lang != language)
                LoadLanguage(lang);
            if (textDic.ContainsKey(_key))
            {
                textDic.Remove(_key);
                SaveText();
            }
        }

        LoadLanguage(currentLang);
    }

    //从字典读取相应文本
    public string GetText(string _key)
    {
        //如果还没初始化，初始化
        if (textDic == null)
        {
            LoadLanguage(language);
        }

        if (textDic.ContainsKey(_key))
            return System.Text.RegularExpressions.Regex.Unescape(textDic[_key]);

        //Debug.LogWarning(_key + "键缺失！");
        return _key.ToString();
    }

    public bool ContainKey(string _key)
    {
        //如果还没初始化，初始化
        if (textDic == null)
        {
            LoadLanguage(language);
        }

        if (textDic.ContainsKey(_key))
            return true;
        return false;
    }
}
