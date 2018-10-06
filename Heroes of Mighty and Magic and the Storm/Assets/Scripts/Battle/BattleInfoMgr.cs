using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoMgr : Singleton<BattleInfoMgr>
{
    public Text text;

    List<string> textList = new List<string>();

    bool tempText;

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

    //手动上移文本
}
