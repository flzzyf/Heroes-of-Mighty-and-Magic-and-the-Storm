using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : NodeObject
{
    public PocketUnit[] pocketUnits;
    [HideInInspector]
    public int movementRate = 1500;
    [HideInInspector]
    public int currentMovementRate;

    public void Init()
    {
        currentMovementRate = movementRate;
    }
}

[System.Serializable]
public class PocketUnit
{
    public UnitType type;
    public int num;
}
