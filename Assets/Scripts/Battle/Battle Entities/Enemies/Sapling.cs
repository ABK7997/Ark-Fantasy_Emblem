using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic magical enemy. Attacks with magic and occasionally heals party members
/// </summary>
public class Sapling : Enemy {

    public override void Behavior()
    {
        string command = "MAGIC";
        int spell;
        Entity target;

        int chance = Random.Range(0, 100);

        //Attack at Random
        if (chance < 70)
        {
            target = GetRandomPlayer();
            spell = ETHEREAL_ARROW;
        }

        //Heal lowest HP party member
        else
        {
            target = GetAllyLowHP();
            if (target != null)
            {
                spell = HEAL;
            }

            //Caster is the last survivor - revert back to normal attack pattern
            else
            {
                target = GetRandomPlayer();
                spell = ETHEREAL_ARROW;
            }
        }

        SetSpecial(spell, Special.CLASS.SPELL);
        party.ExecuteAction(command, this, target);
        ResetTimer();
    }

}
