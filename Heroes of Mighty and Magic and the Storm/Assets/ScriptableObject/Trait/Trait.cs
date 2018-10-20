using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Trait")]
public class Trait : ScriptableObject
{
    [HideInInspector]
    public Language currentLanguage;

    public string traitName
    {
        get
        {
            return LocalizationMgr.instance.GetText(base.name);
        }
    }
}