using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HeroClass")]
public class HeroClass : ScriptableObject
{
    public int att, def, power, knowledge;

    public HeroStatIncreaseRate statRate_before10;
    public HeroStatIncreaseRate statRate_after10;

    [System.Serializable]
    public struct HeroStatIncreaseRate
    {
        public int att, def, power, knowledge;
    }
}
