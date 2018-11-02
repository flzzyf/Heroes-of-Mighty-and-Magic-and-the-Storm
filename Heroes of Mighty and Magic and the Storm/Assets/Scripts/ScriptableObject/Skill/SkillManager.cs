using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    public static void AddSkill(Hero _hero, Skill _skill, int _level)
    {
        Skill skill = _skill;
        skill.level = _level;
        _hero.skills.Add(skill);
    }
}
