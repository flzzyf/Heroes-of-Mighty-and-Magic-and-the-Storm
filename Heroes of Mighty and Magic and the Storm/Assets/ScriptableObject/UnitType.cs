using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public MoveType moveType;
    public AttackType attackType;
    public GameObject missile;
    public List<Trait> traits;
}

public enum MoveType { walk, fly, blink }
public enum AttackType { melee, range }
