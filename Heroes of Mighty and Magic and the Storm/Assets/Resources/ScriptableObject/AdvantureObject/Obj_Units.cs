using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AdvantureObject/Units")]
public class Obj_Units : AdvantureObject
{
    public PocketUnit unit;

    public override void OnInteracted(Hero _hero)
    {
        base.OnInteracted(_hero);

        Debug.Log("进入战斗");
        Hero attacker = TravelManager.instance.currentHero;

        //
        attacker = GameManager.instance.testHeroes[0];

        Hero defender = Instantiate(TravelManager.instance.prefab_hero).GetComponent<Hero>();
        defender.player = 1;
        defender.pocketUnits[0] = unit;

        TravelManager.instance.BattleBegin(attacker, defender);
    }
}
