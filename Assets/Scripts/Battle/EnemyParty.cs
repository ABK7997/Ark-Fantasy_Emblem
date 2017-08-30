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
            party[i].SetParty(this);
        }
    }

    //TODO - HANDLE ENEMIES//
    void Update()
    {
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
                e.Behavior();
                ui.SetProjectionInfo(e.Atk, 100, 0);
                bm.SetState("ENEMY_PROJECTION");

                e.ChangeColor("active");
                target.ChangeColor("target");
            }
        }
    }
}
