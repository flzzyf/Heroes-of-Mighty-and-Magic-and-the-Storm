using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MagicBookMgr : Singleton<MagicBookMgr>
{
    public GameObject ui;
    public MagicItem[] items;

    public GameObject panel_magicInfo;
    public Text text_magicInfo_name;
    public Text text_magicInfo_info;
    public Text text_magicInfo_effect;
    public Image image_magicInfo;
    public Text text_magicInfo_name2;

    //每页的魔法数量
    const int magicsPerPage = 12;

    List<Magic> magicList;

    MagicSchool currentSchool;
    MagicType currentMagicType;
    int currentPage;

    public Button button_all, button_fire, button_water, button_earth, button_air;
    Dictionary<MagicSchool, Button> dic_school_button;
    public Vector2 buttons_school_x;

    public Button button_nextPage;

    Hero currentHero;

    public Text text_info;

    void Start()
    {
        //初始化魔法项的ID
        for (int i = 0; i < items.Length; i++)
        {
            items[i].bookIndex = i;
        }

        dic_school_button = new Dictionary<MagicSchool, Button>();
        dic_school_button.Add(MagicSchool.All, button_all);
        dic_school_button.Add(MagicSchool.Fire, button_fire);
        dic_school_button.Add(MagicSchool.Water, button_water);
        dic_school_button.Add(MagicSchool.Earth, button_earth);
        dic_school_button.Add(MagicSchool.Air, button_air);
    }

    //显示和隐藏UI
    public void Show()
    {
        //根据旅行和战斗模式切换英雄
        //if(GameManager.instance.scene == GameScene.Travel)
        SetMagics(BattleManager.heroes[0]);
        ShowMagics(MagicSchool.All, MagicType.Battle);
        ui.SetActive(true);
    }
    public void Hide()
    {
        ui.SetActive(false);
    }

    void HideMagicItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(false);
        }
    }

    void ShowMagicItem(int _index)
    {
        items[_index].gameObject.SetActive(true);
    }

    //将英雄的魔法加入列表，整理排序
    public void SetMagics(Hero _hero)
    {
        currentHero = _hero;

        magicList = new List<Magic>();

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < _hero.magics.Count; j++)
            {
                if (_hero.magics[j].level == i + 1)
                    magicList.Add(_hero.magics[j]);
            }
        }

        //根据首字排序
        //magicList.Sort(EditorUtility.NaturalCompare);
    }

    //显示魔法，根据派系、是否战斗型、页数
    public void ShowMagics(MagicSchool _school, MagicType _type, int _page = 0)
    {
        currentPage = _page;
        currentSchool = _school;
        currentMagicType = _type;

        //隐藏所有魔法
        HideMagicItems();

        List<Magic> magics = new List<Magic>();
        //根据派系和类型过滤
        foreach (Magic item in magicList)
        {
            if ((_school == MagicSchool.All || item.school == _school || item.school == MagicSchool.All) &&
                (item.type == _type || _type == MagicType.All))
                magics.Add(item);
        }
        //开始的位置
        int startMagicIndex = _page * magicsPerPage;

        //如果还有下页，显示下一页按钮
        if (magics.Count - startMagicIndex > magicsPerPage)
            button_nextPage.gameObject.SetActive(true);
        else
            button_nextPage.gameObject.SetActive(false);

        //显示魔法
        int showItemCount = Mathf.Min(magicsPerPage, magics.Count - startMagicIndex);

        for (int i = 0; i < showItemCount; i++)
        {
            ShowMagicItem(i);
            items[i].Init(magics[startMagicIndex + i]);
        }
    }

    //显示魔法信息栏
    public void ShowMagicInfo(int _index)
    {
        //确认是右键点击
        if (!Input.GetMouseButton(1))
            return;

        panel_magicInfo.SetActive(true);
        text_magicInfo_name.text = items[_index].magic.magicName;
        text_magicInfo_name2.text = items[_index].magic.magicName;
        text_magicInfo_info.text = items[_index].magic.magicInfo;
        text_magicInfo_effect.text = items[_index].magic.magicEffect;

        image_magicInfo.sprite = items[_index].magic.icon;
    }

    public void CastMagic(int _index)
    {
        Hide();

        MagicManager.instance.CastMagic(currentHero, items[_index].magic);
    }

    //点击魔法学派按钮
    public void Button_School(string _school)
    {
        MagicSchool school = (MagicSchool)System.Enum.Parse(typeof(MagicSchool), _school);
        //高亮所选学派
        foreach (MagicSchool item in dic_school_button.Keys)
        {
            Vector3 pos = dic_school_button[item].transform.localPosition;
            if (item == school)
            {
                pos.x = buttons_school_x.x;
                dic_school_button[item].enabled = false;
            }
            else
            {
                pos.x = buttons_school_x.y;
                dic_school_button[item].enabled = true;
            }

            dic_school_button[item].transform.localPosition = pos;
        }

        ShowMagics(school, currentMagicType);
    }

    public void Button_MagicType(string _type)
    {
        MagicType type = (MagicType)System.Enum.Parse(typeof(MagicType), _type);

        if (currentMagicType == type)
            return;

        ShowMagics(currentSchool, type);
    }

    public void NextPage()
    {
        ShowMagics(currentSchool, currentMagicType, currentPage + 1);
    }
}
