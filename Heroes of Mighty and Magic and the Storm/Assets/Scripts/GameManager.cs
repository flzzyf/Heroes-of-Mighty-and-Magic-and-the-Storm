using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool gamePaused;
    public static bool playerControl;

    public AudioSource audioSource;
    public float randomPitch = 0.3f;

    public Hero[] testHeroes;

    public static int player;

    void Start()
    {
        TravelManager.instance.Init();
        BattleManager.instance.Init();

        TravelManager.instance.EnterTravelMode();
        TravelManager.instance.TurnStart(0);

        //添加测试英雄
        PlayerManager.instance.players[0].heroes.Add(testHeroes[0]);
        PlayerManager.instance.players[1].heroes.Add(testHeroes[1]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Hero attacker = PlayerManager.instance.players[0].heroes[1];
            Hero defender = PlayerManager.instance.players[1].heroes[1];
            TravelManager.instance.BattleBegin(attacker, defender);

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            TravelManager.instance.EnterTravelMode();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < BattleResultMgr.instance.parent_units[i].childCount; j++)
                {
                    int childCount = BattleResultMgr.instance.parent_units[i].childCount - 1;
                    if (BattleResultMgr.instance.parent_units[i].GetChild(childCount).gameObject.activeSelf)
                        Destroy(BattleResultMgr.instance.parent_units[i].GetChild(childCount));
                }
            }
            BattleResultMgr.instance.ShowResultUI(0);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            BattleResultMgr.instance.ShowResultUI(1);
        }
    }

    public void PlaySound(AudioClip _clip, bool _random = false)
    {
        if (_random)
            audioSource.pitch = 1 + Random.Range(0, 1f) * randomPitch;

        audioSource.PlayOneShot(_clip);
    }
}
