using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill")]
public class Skill : ScriptableObject
{
    public int level;
    public Sprite icon;
    public float[] amounts = new float[3];
}
