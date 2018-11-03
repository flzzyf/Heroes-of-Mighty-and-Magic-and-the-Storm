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
        print(_magic);

        //判定魔法学派，根据英雄的相应学派等级释放不同效果
        MagicSchool school = _magic.school;
        int magicLevel = SkillManager.LevelOfSkill(_hero, SchoolToSkill(school));

        print(SchoolToSkill(school));

        //目标类型：无目标直接释放
        //有目标，则选择目标
        if (_magic.targetType[magicLevel] == MagicTargetType.Null)
        {

        }
    }

}
