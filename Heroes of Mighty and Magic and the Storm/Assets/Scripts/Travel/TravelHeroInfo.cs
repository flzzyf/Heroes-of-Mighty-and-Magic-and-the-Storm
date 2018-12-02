using System.Collections.Generic;
using UnityEngine;

public class TravelHeroInfo : Singleton<TravelHeroInfo>
{
    public Panel_MoraleAndLuck moraleAndLuck;

    public GameObject ui;

    void Start()
    {
        ui.SetActive(false);
    }

    public void Enter()
    {
        ui.SetActive(true);

        Init(TravelManager.instance.currentHero);
    }

    public void Quit()
    {
        ui.SetActive(false);
    }

    public void Init(Hero _hero)
    {
        moraleAndLuck.SetMorale(_hero.morale);
        moraleAndLuck.SetLuck(_hero.luck);
    }
}
