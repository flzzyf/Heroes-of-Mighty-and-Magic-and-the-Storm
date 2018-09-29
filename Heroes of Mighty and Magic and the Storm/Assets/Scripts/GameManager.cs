using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool gamePaused;

    void Start()
    {
        Cursor.visible = false;

        TravelManager.instance.Init();
        BattleManager.instance.Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            BattleManager.instance.EnterBattle();
            BattleManager.instance.BattleStart();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            TravelManager.instance.EnterTravelMode();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
        }
    }
}
