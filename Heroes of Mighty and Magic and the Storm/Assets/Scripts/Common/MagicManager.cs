using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicManager : MonoBehaviour
{
    public void CastMagic(Hero _hero, Magic _magic)
    {
        MagicSchool school = _magic.school;
        int level = _magic.level;
        if(_magic.targetType[0] == MagicTargetType.Null)
        {

        }
    }
}
