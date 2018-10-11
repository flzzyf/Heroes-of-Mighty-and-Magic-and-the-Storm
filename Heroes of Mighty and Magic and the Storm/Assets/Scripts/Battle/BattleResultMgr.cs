using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultMgr : Singleton<BattleResultMgr>
{
    public GameObject resultUI;
    public Text[] text_heroName;
    public Image[] image_portrait;
    public Text[] text_role;
    public Text[] text_result;
    public Transform[] parent_units;

    public GameObject prefab_unitIcon;

    static string[] roleText = { "胜利", "战败" };
    static string[] info = { "辉煌的胜利！\n因为英勇作战,{0}获得了2000经验值", "战败" };

    //战损记录字典
    Dictionary<UnitType, int>[] casualties;

    public void Init()
    {
        casualties = new Dictionary<UnitType, int>[2];

        for (int i = 0; i < 2; i++)
        {
            casualties[i] = new Dictionary<UnitType, int>();
            for (int j = 0; j < parent_units[i].childCount; j++)
                Destroy(parent_units[i].GetChild(0));
        }
    }

    public void ShowResultUI(int _winningSide)
    {
        UpdateResultUI(_winningSide);
        ShowResultUI();
    }

    public void ShowResultUI(bool _show = true)
    {
        resultUI.SetActive(_show);
    }

    void UpdateResultUI(int _winningSide)
    {
        for (int i = 0; i < 2; i++)
        {
            text_heroName[i].text = BattleManager.heroes[i].heroType.heroName;
            image_portrait[i].sprite = BattleManager.heroes[i].heroType.icon;
            text_role[i].text = roleText[_winningSide == i ? 0 : 1];
            text_result[i].gameObject.SetActive(_winningSide == i ? true : false);
            text_result[i].text = string.Format(info[i], BattleManager.heroes[i].heroType.heroName);

            foreach (var item in casualties[i])
            {
                ResultUnitIcon icon = Instantiate(prefab_unitIcon,
                    parent_units[i]).GetComponent<ResultUnitIcon>();

                icon.icon.sprite = item.Key.icon;
                Vector2 size = icon.icon.GetComponent<RectTransform>().sizeDelta;
                float rate = (float)item.Key.icon.texture.width / item.Key.icon.texture.height;
                size.x = icon.icon.GetComponent<RectTransform>().sizeDelta.y * rate;
                icon.icon.GetComponent<RectTransform>().sizeDelta = size;

                icon.text_num.text = item.Value * -1 + "";
            }
        }

    }

    //添加新的战损记录
    public void ChangeCasualties(int _player, UnitType _type, int _amount)
    {
        //未含有该单位则增加键，已有则增加值
        if (!casualties[_player].ContainsKey(_type))
        {
            casualties[_player].Add(_type, _amount);
        }
        else
        {
            casualties[_player][_type] += _amount;
            //为0清除该键
            if (casualties[_player][_type] == 0)
                casualties[_player].Remove(_type);
        }
    }
}
