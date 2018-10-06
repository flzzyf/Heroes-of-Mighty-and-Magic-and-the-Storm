using UnityEngine;

[CreateAssetMenu(menuName = "Trait")]
public class Trait : ScriptableObject
{
    public string traitName;
    public float chance = 1;

    public AudioClip sound_trigger;
    public GameObject fx_trigger;
}