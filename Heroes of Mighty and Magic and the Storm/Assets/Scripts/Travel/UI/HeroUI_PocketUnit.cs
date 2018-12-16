using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroUI_PocketUnit : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public Image portrait;
    public Text text_num;

    public GameObject border_selected;

    public PocketUnit unit;

    public static HeroUI_PocketUnit selectedPanel;

    void Start()
    {
        border_selected.SetActive(false);
    }

    //设置
    public void Set(PocketUnit _unit)
    {
        unit = _unit;

        if (!portrait.enabled)
            portrait.enabled = true;
        portrait.sprite = _unit.type.icon;
        text_num.text = _unit.num + "";
    }
    public void Refresh()
    {
        text_num.text = unit.num + "";
    }
    //清空
    public void Clear()
    {
        portrait.enabled = false;
        text_num.text = "";

        unit = null;
    }

    //选中
    public void Select()
    {
        Highlight(true);

        selectedPanel = this;
    }
    public void Deselect()
    {
        Highlight(false);

        selectedPanel = null;
    }

    public void Highlight(bool _highlight)
    {
        border_selected.SetActive(_highlight);
    }

    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (unit != null)
            Panel_HeroUI.instance.text_bottomInfo.SetText(LocalizationMgr.instance.GetText("Select") + unit.type.unitName);
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        //未选中物体
        if (selectedPanel == null)
        {
            //且不为空，则选中
            if (unit != null)
                Select();
        }
        else
        {
            //已经选中物体。为空则移动。不为空：不同单位则交换，同种单位叠加
            if (unit == null)
            {
                Set(selectedPanel.unit);
                selectedPanel.Clear();
            }
            else
            {
                if (unit.type != selectedPanel.unit.type)
                {
                    //交换
                    PocketUnit temp = unit;
                    Set(selectedPanel.unit);
                    selectedPanel.Set(temp);
                }
                else
                {
                    //叠加
                    unit.num += selectedPanel.unit.num;
                    Set(unit);
                    selectedPanel.Clear();
                }
            }

            selectedPanel.Deselect();
        }
    }
}
