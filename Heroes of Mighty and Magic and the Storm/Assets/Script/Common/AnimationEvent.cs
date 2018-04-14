using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour {

    public void Event_AttackHit()
    {
        UnitActionManager.instance.Event_Attack();

    }

}
