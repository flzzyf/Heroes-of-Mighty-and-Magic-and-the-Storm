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

}
