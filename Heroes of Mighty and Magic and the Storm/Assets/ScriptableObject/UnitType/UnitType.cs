using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "UnitType")]
public class UnitType : ScriptableObject
{
    public int tab;

    public Sprite icon;
    public Race race;
    public RuntimeAnimatorController animControl;

    public int level = 1;
    public int att, def, hp;
    public Vector2Int damage;
    public int speed = 5;

    public AttackType attackType;
    public List<Trait> traits;

    public int growth;
    public int AIValue;
    public int cost;

    public UnitSize size = UnitSize.middle;
    public ArmorType armorType;
    public bool isTwoHexsUnit = false;

    public int ammo = 12;
    public GameObject missile;
    public Vector2 launchPos;

    public AudioClip sound_attack;
    public AudioClip sound_attackImpact;
    public AudioClip[] sound_walk;
    public FixedSound sound_hit;
    public FixedSound sound_death;

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

    public string unitName
    {
        get
        {
            return LocalizationMgr.instance.GetText(base.name);
        }
    }
}

public enum AttackType { melee, range }
public enum UnitSize { small, middle, big }
public enum ArmorType { flesh, metal }
