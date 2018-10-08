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

    [System.Serializable]
    public class HaloColor
    {
        public string name;
        public Color color = Color.white;
    }

    public HaloColor[] haloColor;

    void ChangeHalo(Unit _unit, float _value = 0)
    {
        print(_value);
        Color color = _unit.sprite.material.GetColor("_Color");
        color.a = _value;
        _unit.sprite.material.SetColor("_Color", color);
    }

    public void HaloFlashStart(Unit _unit)
    {
        _unit.sprite.material.SetFloat("_LineWidth", haloWidth);

        ChangeHalo(_unit, haloFlashRangeMax);

        flashingUnits.Add(_unit);

        StartCoroutine(HaloFlashing(_unit));
    }

    public void HaloFlashStart(Unit _unit, string _color)
    {
        ChangeHaloColor(_unit, _color);
        HaloFlashStart(_unit);
    }

    IEnumerator HaloFlashing(Unit _unit)
    {
        bool fading = true;

        while (flashingUnits.Contains(_unit))
        {
            float a = _unit.sprite.material.GetColor("_Color").a;

            if (a < haloFlashRangeMin) fading = false;
            else if (a > haloFlashRangeMax) fading = true;

            if (fading)
            {
                a -= haloFlashSpeed * Time.deltaTime;
            }
            else
                a += haloFlashSpeed * Time.deltaTime;

            ChangeHalo(_unit, a);

            yield return null;
        }
    }

    public void HaloFlashStop(Unit _unit)
    {
        _unit.sprite.material.SetFloat("_LineWidth", 0);

        flashingUnits.Remove(_unit);
    }

    void ChangeHaloColor(Unit _unit, string _color)
    {
        for (int i = 0; i < haloColor.Length; i++)
        {
            if (_color == haloColor[i].name)
            {
                _unit.sprite.material.SetColor("_Color", haloColor[i].color);
                return;
            }
        }

        Debug.LogWarning("未能找到颜色");
    }
}
