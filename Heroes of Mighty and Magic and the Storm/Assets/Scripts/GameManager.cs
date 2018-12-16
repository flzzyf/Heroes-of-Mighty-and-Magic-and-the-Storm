using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameScene { Travel, Battle }

public enum GameState { playerControl, canNotControl }

public class GameManager : Singleton<GameManager>
{
    public static GameState gameState;

    public Hero[] testHeroes;

    public int player = 2;
    public static Player currentPlayer;

    public GameScene scene;

    void Start()
    {
        TravelManager.instance.Init();
        BattleManager.instance.Init();

        //之前有保存语言则直接设置，否则根据本地语言设置
        // if (PlayerPrefs.HasKey("Language"))
        //     ChangeLanguage(PlayerPrefs.GetString("Language"));
        // else
        //     ChangeToLocalLanguage();

        PlayerManager.instance.players[0].heroes.Add(testHeroes[0]);
        PlayerManager.instance.players[1].heroes.Add(testHeroes[1]);

        //给测试英雄加上所有魔法
        MagicManager.AddAllMagic(testHeroes[0]);

        AdvantureObjectMgr.CreateAdvantureObject("Chest", new Vector2Int(23, 19));
        AdvantureObjectMgr.CreateAdvantureObject("Leorics", new Vector2Int(19, 17));
        AdvantureObjectMgr.CreateAdvantureObject("Chest", new Vector2Int(23, 14));
        AdvantureObjectMgr.CreateAdvantureObject("Gold", new Vector2Int(19, 14));
        AdvantureObjectMgr.CreateAdvantureObject("Gold", new Vector2Int(17, 14));
        AdvantureObjectMgr.CreateAdvantureObject("Gold", new Vector2Int(15, 14));

        SkillManager.AddSkill(testHeroes[0], "Magic_Air", 2);
        SkillManager.AddSkill(testHeroes[0], "Magic_Fire", 2);
        SkillManager.AddSkill(testHeroes[0], "Magic_Earth", 2);
        SkillManager.AddSkill(testHeroes[0], "Magic_Water", 2);

        TravelManager.instance.EnterTravelMode();
        TravelManager.instance.TurnStart(GameManager.instance.player);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Hero attacker = testHeroes[0];
            Hero defender = testHeroes[1];
            TravelManager.instance.BattleBegin(attacker, defender);

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SoundManager.instance.PlaySound("Combat02");
            //TravelManager.instance.EnterTravelMode();
            //LocalizationMgr.instance.ChangeToLanguage(Language.English);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SoundManager.instance.StopPlay("Combat02");
            //LocalizationMgr.instance.ChangeToLanguage(Language.Chinese_Simplified);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SoundManager.instance.PlaySound("PickUp");
            //SkillManager.AddSkill(testHeroes[0], "Magic_Air", 3);
            //MagicBookMgr.instance.SetMagics(testHeroes[0]);
            //MagicBookMgr.instance.ShowMagics(MagicSchool.All, MagicType.Battle);
            //BattleResultMgr.instance.ShowResultUI(0);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SoundManager.instance.StopPlay("PickUp");
            //MagicBookMgr.instance.ShowMagics(MagicSchool.All, MagicType.All, 1);

            //MagicBookMgr.instance.Show();
            //BattleResultMgr.instance.ShowResultUI(1);
            //Resources.LoadAll<Sprite>("Textures");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SkillManager.AddSkill(PlayerManager.instance.players[0].heroes[0], "Sorcery", 1);
            SkillManager.AddSkill(PlayerManager.instance.players[0].heroes[0], "Wisdom", 2);
            SkillManager.AddSkill(PlayerManager.instance.players[0].heroes[0], "Magic_Air", 2);
            SkillManager.AddSkill(PlayerManager.instance.players[0].heroes[0], "Magic_Fire", 2);
            SkillManager.AddSkill(PlayerManager.instance.players[0].heroes[0], "Magic_Water", 2);
            SkillManager.AddSkill(PlayerManager.instance.players[0].heroes[0], "Magic_Earth", 2);

            //SkillManager.AddSkill(testHeroes[0], "Magic_Earth", 3);
            //MagicManager.instance.CastMagic(testHeroes[0], testHeroes[0].magics[1]);

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SoundManager.instance.PlaySound("Combat02");
        }
    }

    //变为本地语言
    void ChangeToLocalLanguage()
    {
        string localLang = Application.systemLanguage.ToString();
        //print("本地语言：" + localLang);
        ChangeLanguage(localLang);
    }

    void ChangeLanguage(string _lang)
    {
        if (_lang == "Chinese")
            LocalizationMgr.instance.ChangeToLanguage(Language.Chinese_Simplified);
        else
            LocalizationMgr.instance.ChangeToLanguage(Language.English);
    }
}
