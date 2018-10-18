using UnityEngine;
using UnityEngine.UI;

public class LocalizationFont : MonoBehaviour
{
    void OnEnable()
    {
        LocalizationMgr.instance.localizationFonts.Add(this);
        Init();
    }

    public void Init()
    {
        GetComponent<Text>().font = LocalizationMgr.instance.font;
    }
}
