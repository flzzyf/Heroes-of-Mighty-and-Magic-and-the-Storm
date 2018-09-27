using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool gamePaused;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            BattleManager.instance.BattleStart();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            BattleManager.instance.EnterBattle();
        }
    }
}
