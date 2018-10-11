using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait_Effect")]
public class Trait_Effect : Trait
{
    public float chance = 1;
    public AudioClip sound_trigger;
    public GameObject fx_trigger;
}
