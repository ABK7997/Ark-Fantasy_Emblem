//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy {

    private const int SHOCK = 0;

    public override void Behavior()
    {
        base.Behavior();

        Entity target = GetRandomPlayer();

        //Even turns - attack with Tech
        if (turn % 2 == 0)
        {
            SetSpecial(SHOCK, Special.CLASS.TECH);
            party.ExecuteAction(Party.COMMAND.TECH, this, target);
        }

        //Odd turns - physical attack
        else
        {
            party.ExecuteAction(Party.COMMAND.ATTACK, this, target);
        }
    }

}
