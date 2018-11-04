using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Trait")]
public class Trait : ScriptableObject
{
    public string traitName
    {
        get
        {
            return LocalizationMgr.instance.GetText(base.name);
        }
    }

}