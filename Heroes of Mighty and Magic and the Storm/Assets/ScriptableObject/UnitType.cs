using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "UnitType")]
public class UnitType : ScriptableObject
{
    public string unitName;
    public Sprite icon;
    public RuntimeAnimatorController animControl;

    public int att, def, hp;
    public Vector2Int damage;
    public int speed = 5;

    public AttackType attackType;
    public List<Trait> traits;

    [Header("每周产量")]
    public int growth;
    public int AIValue;
    public int cost;

    public UnitSize size = UnitSize.middle;
    public ArmorType armorType;

    [Header("远程攻击才需要")]
    public int ammo = 12;
    public GameObject missile;
    public Vector2 launchPos;
    [Header("音效")]
    public AudioClip sound_attack;
    public AudioClip sound_attackImpact;
    public AudioClip[] sound_walk;
    public AudioClip sound_hit;
    public AudioClip sound_death;

    public Vector2 impactPos
    {
        get
        {
            Vector3 pos = Vector3.up;
            if (size == UnitSize.small) return pos;
            else if (size == UnitSize.big) return pos * 3;
            return pos * 2;
        }
    }
}

public enum AttackType { melee, range }
public enum UnitSize { small, middle, big }
public enum ArmorType { flesh, metal }
