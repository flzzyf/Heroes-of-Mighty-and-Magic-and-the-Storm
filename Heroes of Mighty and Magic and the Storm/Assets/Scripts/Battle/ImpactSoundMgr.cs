using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundMgr
{
    public static void PlayImpactSound(Unit _origin, Unit _target)
    {
        string armor = "metal", size = "middle";

        if (_origin.type.size == UnitSize.small) size = "small";
        else if (_origin.type.size == UnitSize.middle) size = "middle";
        else if (_origin.type.size == UnitSize.big) size = "big";

        if (_target.type.armorType == ArmorType.flesh) armor = "flesh";
        else if (_target.type.armorType == ArmorType.metal) armor = "metal";

        SoundManager.instance.PlaySound(string.Format("impact_{0}_{1}", size, armor));
    }
}
