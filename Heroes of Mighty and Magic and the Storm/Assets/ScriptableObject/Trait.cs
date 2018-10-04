using UnityEngine;

[CreateAssetMenu(menuName = "Trait")]
public class Trait : ScriptableObject
{
    public new string name;
    public float chance = 1;
}