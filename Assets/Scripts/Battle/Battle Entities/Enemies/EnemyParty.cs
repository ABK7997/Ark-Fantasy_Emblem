using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The party containing the enemies the player's party is currently fighting
/// </summary>
public class EnemyParty : Party {

    //TODO - HANDLE ENEMIES//
    protected override void Update()
    {
        if (bm == null) return; //Battle Manager not set

        base.Update();

        switch (bm.GetState())
        {
            case "ENEMY_PROJECTION":

                target.ChangeColor("target");
                activeMember.ChangeColor("active");

                break;
        }

        foreach (Enemy e in party)
        {
            if (e.Ready && bm.GetState() == "NORMAL")
            {
                command = COMMAND.ATTACK;
                otherPartyBody.ResetPositionAll();
                e.Behavior();
                bm.SetState("ENEMY_PROJECTION");
                CalculateAction();

                e.ChangeColor("active");
                target.ChangeColor("target");

                break; //Exit loop after first entity acts
            }
        }
    }
}
