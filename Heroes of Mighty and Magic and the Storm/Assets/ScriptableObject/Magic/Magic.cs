using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicSchool { All, Earth, Fire, Air, Water}

[CreateAssetMenu(menuName = "Magic")]
public class Magic : ScriptableObject
{
    public Sprite icon;
    public MagicSchool school;
    public int level = 1;
    public int[] mana = new int[1];

    //目标类型
    public Effect effect;

    public string magicName { get { return LocalizationMgr.instance.GetText(base.name); } }
}
