using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    //添加技能到英雄
    public static void AddSkill(Hero _hero, string _skill, int _level)
    {
        Skill skill = GetSkill(_skill);
        skill.level = _level;
        _hero.skills.Add(skill);
    }

    //判定有该技能
    public static bool HasSkill(Hero _hero, string _skill, int _level)
    {
        Skill skill = GetSkill(_skill);
        foreach (Skill item in _hero.skills)
        {
            if (item == skill && item.level == _level)
                return true;
        }
        return false;
    }
    //获取技能等级
    public static int LevelOfSkill(Hero _hero, Skill _skill)
    {
        foreach (Skill item in _hero.skills)
        {
            if (item == _skill)
                return item.level;
        }

        return 0;
    }
    public static int LevelOfSkill(Hero _hero, string _skill)
    {
        return LevelOfSkill(_hero, GetSkill(_skill));
    }

    //根据名字获取技能
    public static Skill GetSkill(string _name)
    {
        Skill[] skills = Resources.LoadAll<Skill>("ScriptableObject/Skill/Instance");

        foreach (Skill item in skills)
        {
            if (item.name == _name)
                return item;
        }

        return null;
    }
}
