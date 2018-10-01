using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : Singleton<GameSettings>
{
    [Range(0, 1)]
    public float outlineFlashRangeMin;
    [Range(0, 1)]
    public float outlineFlashRangeMax;

    public float outlineFlashSpeed = 1;

}
