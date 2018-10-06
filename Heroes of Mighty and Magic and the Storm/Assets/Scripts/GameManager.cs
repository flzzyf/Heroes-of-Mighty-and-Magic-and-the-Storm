using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool gamePaused;

    public AudioSource audioSource;

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
        if (Input.GetKeyDown(KeyCode.V))
        {
            BattleInfoMgr.instance.AddText("qwe");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            BattleInfoMgr.instance.AddText("asd");
        }
    }

    public void PlaySound(AudioClip _clip)
    {
        audioSource.PlayOneShot(_clip);
    }
}
