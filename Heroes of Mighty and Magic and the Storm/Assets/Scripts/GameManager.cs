using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameScene { Travel, Battle }

public enum GameState { playerControl, canNotControl, paused }

public class GameManager : Singleton<GameManager>
{
    public static GameState gameState;

    public AudioSource audioSource;
    public float randomPitch = 0.3f;

    public Hero[] testHeroes;

    public static int player;

    public Item item;

    public GameScene scene;

    void Start()
    {
        TravelManager.instance.Init();
        BattleManager.instance.Init();

        TravelManager.instance.EnterTravelMode();
        TravelManager.instance.TurnStart(0);

        //之前有保存语言则直接设置，否则根据本地语言设置
        // if (PlayerPrefs.HasKey("Language"))
        //     ChangeLanguage(PlayerPrefs.GetString("Language"));
        // else
        //     ChangeToLocalLanguage();

        ItemManager.instance.CreateItem(item, new Vector2Int(23, 19));
        ItemManager.instance.CreateItem(item, new Vector2Int(23, 14));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Hero attacker = PlayerManager.instance.players[0].heroes[0];
            Hero defender = PlayerManager.instance.players[1].heroes[0];
            TravelManager.instance.BattleBegin(attacker, defender);

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            //TravelManager.instance.EnterTravelMode();
            LocalizationMgr.instance.ChangeToLanguage(Language.English);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            LocalizationMgr.instance.ChangeToLanguage(Language.Chinese_Simplified);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SkillManager.AddSkill(testHeroes[0], "Magic_Air", 3);
            MagicBookMgr.instance.SetMagics(testHeroes[0]);
            MagicBookMgr.instance.ShowMagics(MagicSchool.All, MagicType.Battle);
            //BattleResultMgr.instance.ShowResultUI(0);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            MagicBookMgr.instance.ShowMagics(MagicSchool.All, MagicType.All, 1);

            //MagicBookMgr.instance.Show();
            //BattleResultMgr.instance.ShowResultUI(1);
            //Resources.LoadAll<Sprite>("Textures");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SkillManager.AddSkill(testHeroes[0], "Magic_Earth", 3);
            MagicManager.instance.CastMagic(testHeroes[0], testHeroes[0].magics[1]);

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SkillManager.AddSkill(testHeroes[0], "Magic_Fire", 3);
            print(SkillManager.HasSkill(testHeroes[0], "Magic_Fire", 3));
        }
    }

    public void PlaySound(FixedSound _sound, bool _random = false)
    {
        if (_sound == null)
            return;


        audioSource.clip = _sound.clip;
        audioSource.time = _sound.skipDuration;
        audioSource.Play();
    }

    public void PlaySound(AudioClip _clip, bool _random = false)
    {
        if (_clip == null)
            return;

        if (_random)
            audioSource.pitch = 1 + Random.Range(0, 1f) * randomPitch;

        audioSource.PlayOneShot(_clip);
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

[System.Serializable]
public class FixedSound
{
    public AudioClip clip;
    public float skipDuration;
}
