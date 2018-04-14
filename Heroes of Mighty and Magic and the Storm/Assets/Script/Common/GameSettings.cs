using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour 
{
    #region Singleton
    [HideInInspector]
    public static GameSettings instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion

    [Range(0, 1)]
    public float outlineFlashRangeMin;
    [Range(0, 1)]
    public float outlineFlashRangeMax;

    public float outlineFlashSpeed = 1;

}
