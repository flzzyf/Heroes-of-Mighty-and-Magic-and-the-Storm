﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoMgr : Singleton<BattleInfoMgr>
{
    public Text text;

    List<string> textList = new List<string>();

    bool tempText = true;

    int currentPage;

    public void SetText(string _text)
    {
        tempText = true;

        text.text = _text;
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
            text.text = textList[textList.Count - 2] + "\n" + _text;

            currentPage = 0;
        }
    }

    public void ClearText()
    {
        text.text = "";
    }

    IEnumerator AutoClearText()
    {
        yield return new WaitForSeconds(3);
        ClearText();
    }

    public void AddText_Damage(Unit _origin, Unit _target, int _damage, int _deathNum)
    {
        string text = string.Format("{0}造成{1}点伤害", _origin.type.unitName, _damage);
        if (_damage >= _target.type.hp)
            text += string.Format("，{0}个{1}死了", _deathNum, _target.type.unitName);

        AddText(text);
    }

    public void AddText_LifeDrain(Unit _origin, Unit _target, int _damage, int _resurrectNum)
    {
        string text = string.Format("{0}从{1}吸收了{2}点生命", _origin.type.unitName,
            _target.type.unitName, _damage);
        if (_resurrectNum > 0)
            text += string.Format("，并有{0}个复活了", _resurrectNum);

        AddText(text);
    }

    public void ButtonUp()
    {
        if (currentPage < textList.Count - 2)
        {
            currentPage++;
            string s = textList[textList.Count - currentPage - 2] + "\n" +
                        textList[textList.Count - currentPage - 1];
            text.text = s;
        }
    }

    public void ButtonDown()
    {
        if (currentPage > 0)
        {
            currentPage--;
            string s = textList[textList.Count - currentPage - 2] + "\n" +
                        textList[textList.Count - currentPage - 1];
            text.text = s;
        }
    }

    //手动上移文本
}
