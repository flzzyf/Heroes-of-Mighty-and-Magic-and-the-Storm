﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoMgr : Singleton<BattleInfoMgr>
{
    public Text text, text2;

    List<string> textList = new List<string>();

    bool tempText = true;

    int currentPage;

    public void SetText(string _text)
    {
        tempText = true;

        text.text = _text;
        text2.text = "";
    }
    public void SetTextByKey(string _key)
    {
        tempText = true;

        text.text = LocalizationMgr.instance.GetText(_key);
        text2.text = "";
    }

    public void AddText(string _text)
    {
        if (tempText)
        {
            tempText = false;
            ClearText();
        }

        textList.Add(_text);

        //如果此时没有显示文本，直接显示
        if (text.text == "")
        {
            text.text = _text;
        }
        else
        {
            //已有显示文本，则加进去
            text.text = textList[textList.Count - 2];
            text2.text = _text;

            currentPage = 0;
        }
    }

    public void ClearText()
    {
        text.text = "";
        text2.text = "";
    }

    IEnumerator AutoClearText()
    {
        yield return new WaitForSeconds(3);
        ClearText();
    }

    public void SetText_Attack(Unit _origin, Unit _target)
    {
        bool isRangeAttack = UnitActionMgr.IsRangeAttack(_origin);
        Vector2Int range = UnitAttackMgr.instance.GetDamageRange(_origin, _target, isRangeAttack);
        string s;
        if (range.x == range.y)
            s = string.Format(LocalizationMgr.instance.GetText("battleInfo_attack"),
                _target.type.unitName, range.x);
        else
            s = string.Format(LocalizationMgr.instance.GetText("battleInfo_attack_range"),
                _target.type.unitName, range.x, range.y);

        SetText(s);
    }

    public void AddText_Damage(Unit _origin, Unit _target, int _damage, int _deathNum)
    {
        string text;
        //如果是英文，单复数判定
        if (LocalizationMgr.instance.language == Language.English)
        {
            bool plural = _origin.num > 1;
            string verb = plural ? "do" : "does";
            string name;
            if (plural)
            {
                if (LocalizationMgr.instance.GetText(_origin.type.name + "_plural") != null)
                {
                    name = LocalizationMgr.instance.GetText(_origin.type.name + "_plural");
                }
                else
                    name = _origin.type.unitName + "s";

            }
            else
                name = _origin.type.unitName;

            text = string.Format(LocalizationMgr.instance.GetText("battleInfo_damage"),
                            name, verb, _damage);
        }
        else
        {
            text = string.Format(LocalizationMgr.instance.GetText("battleInfo_damage"),
            _origin.type.unitName, _damage);
        }

        if (_deathNum > 0)
            text += string.Format(LocalizationMgr.instance.GetText("battleInfo_death"),
                _deathNum, _target.type.unitName);

        AddText(text);
    }

    public void AddText_LifeDrain(Unit _origin, Unit _target, int _damage, int _resurrectNum)
    {
        string text = string.Format(LocalizationMgr.instance.GetText("battleInfo_lifeDrain"),
            _origin.type.unitName, _target.type.unitName, _damage);
        if (_resurrectNum > 0)
            text += string.Format(LocalizationMgr.instance.GetText("battleInfo_lifeDrain_resurrection"),
                _resurrectNum);

        AddText(text);
    }

    public void ButtonUp()
    {
        if (currentPage < textList.Count - 2)
        {
            currentPage++;

            text.text = textList[textList.Count - currentPage - 2];
            text2.text = textList[textList.Count - currentPage - 1];
        }
    }

    public void ButtonDown()
    {
        if (currentPage > 0)
        {
            currentPage--;

            text.text = textList[textList.Count - currentPage - 2];
            text2.text = textList[textList.Count - currentPage - 1];
        }
    }
}
