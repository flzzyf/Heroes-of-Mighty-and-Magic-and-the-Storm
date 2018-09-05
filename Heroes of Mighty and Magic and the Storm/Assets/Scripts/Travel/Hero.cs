using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : NodeObject
{
    public PocketUnit[] pocketUnits;
}

[System.Serializable]
public class PocketUnit
{
    public UnitType type;
    public int num;
}
