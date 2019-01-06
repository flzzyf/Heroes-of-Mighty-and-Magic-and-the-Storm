using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHaloMgr : Singleton<UnitHaloMgr>
{
    [Range(0, 1)]
    public float haloFlashRangeMin = 0.2f;
    [Range(0, 1)]
    public float haloFlashRangeMax = 0.7f;

    public float haloFlashSpeed = 1;

    public float haloWidth = 14;

    List<Unit> flashingUnits = new List<Unit>();

	const string outlineWidth = "_OutlineWidth";
	const string outlineColor = "_OutlineColor";

	[System.Serializable]
    public class HaloColor
    {
        public string name;
        public Color color = Color.white;
    }

    public HaloColor[] haloColor;

    public Shader shader_outline;
    public Shader shader_normal;

    //光晕闪烁开始
    public void HaloFlashStart(Unit _unit)
    {
        if (flashingUnits.Contains(_unit))
            return;

        flashingUnits.Add(_unit);
        StartCoroutine(HaloFlashing(_unit));
    }
    //光晕闪烁停止
    public void HaloFlashStop(Unit _unit)
    {
        flashingUnits.Remove(_unit);
    }
    //光晕以指定颜色开始闪烁
    public void HaloFlashStart(Unit _unit, string _color)
    {
        ChangeHaloColor(_unit, _color);
        HaloFlashStart(_unit);
    }

    IEnumerator HaloFlashing(Unit _unit)
    {
        //设置Shader
        //_unit.sprite.material.shader = shader_outline;

        _unit.sprite.material.SetFloat(outlineWidth, haloWidth);

        int alphaChangingSign = -1;
        float a = haloFlashRangeMax;

        while (flashingUnits.Contains(_unit))
        {
            if (a < haloFlashRangeMin) alphaChangingSign = 1;
            else if (a > haloFlashRangeMax) alphaChangingSign = -1;

            a += alphaChangingSign * haloFlashSpeed * Time.deltaTime;

            ChangeHaloAlpha(_unit, a);

            yield return null;
        }

        _unit.sprite.material.SetFloat(outlineWidth, 0);
        //设置Shader
        //_unit.sprite.material.shader = shader_normal;
    }
    //改变光晕透明度
    void ChangeHaloAlpha(Unit _unit, float _value)
    {
        Color color = _unit.sprite.material.GetColor(outlineColor);
        color.a = _value;
        _unit.sprite.material.SetColor(outlineColor, color);
    }
    //改变光晕颜色
    void ChangeHaloColor(Unit _unit, string _color)
    {
        for (int i = 0; i < haloColor.Length; i++)
        {
            if (_color == haloColor[i].name)
            {
                _unit.sprite.material.SetColor(outlineColor, haloColor[i].color);
                return;
            }
        }

        Debug.LogWarning("未能找到颜色");
    }
}
