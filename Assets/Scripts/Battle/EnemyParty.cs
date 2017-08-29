using UnityEngine;
using System.Collections;

/// <summary>
/// The party containing the enemies the player's party is currently fighting
/// </summary>
public class EnemyParty : Party {

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
}
