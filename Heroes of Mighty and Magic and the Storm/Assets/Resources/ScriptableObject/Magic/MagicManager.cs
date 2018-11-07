using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicManager : Singleton<MagicManager>
{
    //魔法学派对应技能
    Skill SchoolToSkill(MagicSchool _school)
    {
        //如果是全派系通用魔法，则遍历所有派系选出等级最高的
        //if(_school == MagicSchool.All)
        return SkillManager.GetSkill("Magic_" + _school.ToString());
    }

    //释放魔法
    public void CastMagic(Hero _hero, Magic _magic)
    {
        //判定魔法学派，根据英雄的相应学派等级释放不同效果
        MagicSchool school = _magic.school;
        int magicLevel = SkillManager.LevelOfSkill(_hero, SchoolToSkill(school));

        MagicType type = _magic.type;

        if (type == MagicType.Battle)
        {

        }

        //print(SchoolToSkill(school));

        //魔法消耗
        int manaLevel = Mathf.Min(_magic.mana.Length - 1, magicLevel);
        int mana = _magic.mana[manaLevel];

        //目标类型：无目标直接释放
        //有目标，则选择目标
        int targetType = Mathf.Min(_magic.targetType.Length - 1, magicLevel);
        int effect = Mathf.Min(_magic.effects.Length - 1, magicLevel);
        if (_magic.targetType[targetType] == MagicTargetType.Null)
        {
            _magic.effects[effect].originPlayer = _hero.player;
            _magic.effects[effect].Invoke();
        }
    }

    //给英雄添加魔法
    public static void AddMagic(Hero _hero, Magic _magic)
    {
        //如果没有该魔法才添加
        if (!_hero.magics.Contains(_magic))
            _hero.magics.Add(_magic);
    }

    //给英雄添加所有魔法
    public static void AddAllMagic(Hero _hero)
    {
        Magic[] magic = Resources.LoadAll<Magic>("ScriptableObject/Magic/Instance");
        foreach (Magic item in magic)
        {
            _hero.magics.Add(item);
        }
    }

    public static Magic GetMagic(string _name)
    {
        Magic[] magic = Resources.LoadAll<Magic>("ScriptableObject/Magic/Instance");
        foreach (Magic item in magic)
        {
            if (item.name == _name)
                return item;
        }

        return null;
    }

}
