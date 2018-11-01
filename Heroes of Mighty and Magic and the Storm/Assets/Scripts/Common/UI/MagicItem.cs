using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicItem : MonoBehaviour
{
    public Image icon;
    //public int bookIndex;

    public Text text_name;
    public Text text_level;
    public Text text_mana;

    public Magic magic;

    public string key_level;
    public string key_mana;

    public void Init()
    {
        icon.sprite = magic.icon;
        text_name.text = magic.magicName;
        text_level.text = string.Format(LocalizationMgr.instance.GetText(key_level), magic.level.ToString());
        text_mana.text = string.Format(LocalizationMgr.instance.GetText(key_mana), magic.mana[0].ToString());
    }

    public void Init(Magic _magic)
    {
        magic = _magic;
        Init();
    }
}
