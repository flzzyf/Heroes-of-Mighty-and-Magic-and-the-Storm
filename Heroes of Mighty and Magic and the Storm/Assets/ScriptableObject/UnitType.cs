using UnityEngine;

[CreateAssetMenu(menuName = "UnitType")]
public class UnitType : ScriptableObject
{
    public RuntimeAnimatorController animControl;

    public int speed = 5;
    public int att, def, hp;
    [Header("每周产量")]
    public int growth;
    public int cost;
    public int AIValue;
    public Vector2Int damage;
    [Header("反击次数")]
    public int retaliations = 1;
    public MoveType moveType;
    public AttackType attackType;
    //特殊能力
}

public enum MoveType { walk, fly, blink }
public enum AttackType { melee, range }
