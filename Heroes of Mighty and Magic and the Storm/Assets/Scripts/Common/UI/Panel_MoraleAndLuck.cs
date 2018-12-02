using System.Collections.Generic;
using UnityEngine;

public class Panel_MoraleAndLuck : MonoBehaviour
{
    //士气，从-3到+3，对应0到7
    public GameObject[] morales;
    GameObject currentMorale;
    //运气，0到+3
    public GameObject[] lucks;
    GameObject currentLuck;

    void Awake()
    {
        for (int i = 0; i < morales.Length; i++)
        {
            morales[i].SetActive(false);
        }
        for (int i = 0; i < lucks.Length; i++)
        {
            lucks[i].SetActive(false);
        }
    }

    //设置士气，输入-3到+3
    public void SetMorale(int _level)
    {
        if (currentMorale != null)
            currentMorale.SetActive(false);

        _level += 3;
        currentMorale = morales[_level];
        currentMorale.SetActive(true);
    }
    //	设置运气
    public void SetLuck(int _level)
    {
        if (currentLuck != null)
            currentLuck.SetActive(false);

        currentLuck = lucks[_level];
        currentLuck.SetActive(true);
    }

}
