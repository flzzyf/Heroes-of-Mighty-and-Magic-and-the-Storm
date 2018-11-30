using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundMgr
{
    public static void PlayImpactSound(Unit _origin, Unit _target)
    {
        string armor = "Metal", size = "Middle";

        if (_origin.type.size == UnitSize.small) size = "Small";
        else if (_origin.type.size == UnitSize.middle) size = "Middle";
        else if (_origin.type.size == UnitSize.big) size = "Big";

        if (_target.type.armorType == ArmorType.flesh) armor = "Flesh";
        else if (_target.type.armorType == ArmorType.metal) armor = "Metal";

        SoundManager.instance.PlaySound(string.Format("Impact_{0}_{1}", size, armor));
    }
}
