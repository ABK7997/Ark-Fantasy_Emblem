using UnityEngine;
using System.Collections;

/// <summary>
/// The party containing the enemies the player's party is currently fighting
/// </summary>
public class EnemyParty : Party {

    /// <summary>
    /// The player's party
    /// </summary>
    public PlayerParty pParty;

    //The status of the enemy's actions
    private enum STATE
    {
        IDLE, ENEMY_PROJECTION
    }
    private STATE state = STATE.IDLE;

    /// <summary>
    /// Behaves very similary to the quivalent method of the PlayerParty class
    /// </summary>
    public override void OrganizeParty()
    {
        for (int i = 0; i < party.Count; i++)
        {
            Instantiate(party[i], new Vector3(2, 2-i), Quaternion.identity, transform);
        }

        Enemy[] members = FindObjectsOfType<Enemy>();

        for (int i = 0; i < party.Count; i++)
        {
            party[i] = members[i];
            party[i].setParty(this);
        }
    }

    //TODO - HANDLE ENEMIES//
    void Update()
    {
        switch (state)
        {
            case STATE.ENEMY_PROJECTION:

                target.changeColor(Color.red);
                activeMember.changeColor(Color.green);

                break;
        }

        foreach (Enemy e in party)
        {
            if (e.Ready)
            {
                e.behavior();
                bm.setProjectionInfo(e.atk, 100, 0);
                e.changeColor(Color.green);

                pParty.enemyMove();

                target.changeColor(Color.red);

                state = STATE.ENEMY_PROJECTION;
            }
        }
    }

    /// <summary>
    /// Set enemy party's state back to IDLE
    /// </summary>
    public void resetState()
    {
        state = STATE.IDLE;

        if (activeMember != null) activeMember.changeColor(Color.white);
        activeMember = null;

        if (target != null) target.changeColor(Color.white);
        target = null;
    }
}
