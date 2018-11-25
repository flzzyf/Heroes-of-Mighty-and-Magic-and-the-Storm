using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BottomTextSetter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string key;
    public LocalizationText text;

    public void OnPointerEnter(PointerEventData data)
    {
        text.SetText(key);
    }

    public void OnPointerExit(PointerEventData data)
    {
        text.ClearText();
    }
}
