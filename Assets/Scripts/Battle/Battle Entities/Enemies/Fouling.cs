using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fouling : Enemy {

    public override void Behavior()
    {
        base.Behavior();

        Entity target = GetRandomPlayer();

        party.ExecuteAction(Party.COMMAND.ATTACK, this, target);
    }

}
