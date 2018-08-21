using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitType")]
public class UnitType : ScriptableObject {

    public RuntimeAnimatorController animControl;

    public int speed = 5;
    public int att, def, hp, amount, price;
    public Vector2 damage;
    public int fightBackCount = 1;

}
