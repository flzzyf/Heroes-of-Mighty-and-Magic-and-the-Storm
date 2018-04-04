using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

    public PocketUnit[] pocketUnits;

	void Start () {
		
	}
	

}

[System.Serializable]
public class PocketUnit
{
    public UnitType type;
    public int num;
}
