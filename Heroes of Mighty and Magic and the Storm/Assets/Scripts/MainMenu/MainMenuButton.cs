using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite bg;
    public string text;

    public Image bgImage;
    public Image bgOutline;
    public Text textComponent;
    public Outline textOutline;

    void Start()
    {
        Highlight(false);
    }

    public void Highlight(bool _hightlight)
    {
        bgOutline.gameObject.SetActive(_hightlight);
        textOutline.enabled = _hightlight;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight(false);
    }
}
