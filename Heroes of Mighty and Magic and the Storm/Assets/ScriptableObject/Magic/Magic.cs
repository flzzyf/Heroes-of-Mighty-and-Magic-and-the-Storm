using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicSchool { All, Earth, Fire, Air, Water }
public enum MagicTargetType { Null, Unit, Node }
public enum MagicTargetFliter { All, Ally, Enemy }

[CreateAssetMenu(menuName = "Magic")]
public class Magic : ScriptableObject
{
    public Sprite icon;
    public MagicSchool school;
    public int level = 1;
    public int[] mana = new int[1];

    //目标类型
    public MagicTargetType[] targetType = new MagicTargetType[1];
    public MagicTargetFliter[] targetFliter = new MagicTargetFliter[1];

    public Effect effect;

    public string magicName { get { return LocalizationMgr.instance.GetText(base.name); } }
    public string magicInfo { get { return LocalizationMgr.instance.GetText(base.name + "_Info"); } }
    public string magicEffect { get { return LocalizationMgr.instance.GetText(base.name + "_Effect"); } }
}
