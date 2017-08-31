using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Subclass of Party which contains the Player's characters
/// </summary>
public class PlayerParty : Party {

    //Monitors the player's actions in battle
    void Update()
    {
        base.Start();

        //Clicking Left Mouse
        if (Input.GetMouseButtonDown(0))
        {
            switch(bm.GetState())
            {
                //Activate Commands Menu
                case "NORMAL":
                    Entity e = GetPartyMember();
                    if (e != null && e.Ready && !bm.IsAnimating())
                    {
                        activeMember = e;
                        bm.SetState("COMMANDING"); //Pause Game
                    }
                    break;

                //Select Target
                case "SELECTION":
                    Entity t = GetAnyMember();
                    if (t != null)
                    {
                        target = t;
                        activeMember.ChangeColor("active");

                        CalculateAction("ATTACK"); //Battle Projection calculation
                        bm.SetState("PLAYER_PROJECTION");

                        ExecuteAction();
                    }
                    break;

                case "PLAYER_PROJECTION":
                    if (target.IsHovering()) //Double click to perform action immediately
                    {
                        bm.SetState("NORMAL");
                    }
                    break;

                default: break;
            }
        }

        //Other
        switch (bm.GetState())
        {
            case "COMMANDING":
                activeMember.ChangeColor("active");
                break;

            case "SELECTION":
                activeMember.ChangeColor("active");
                break;

            case "PLAYER_PROJECTION":
                activeMember.ChangeColor("active");
                target.ChangeColor("target");
                break;
        }
    }

    /// <summary>
    /// Called by BattleManager to setup party configuration on the board
    /// </summary>
    public override void OrganizeParty()
    {
        for (int i = 0; i < party.Count; i++)
        {
            Instantiate(party[i], new Vector3(0, 2-i), Quaternion.identity, transform);
        }

        PartyMember[] members = FindObjectsOfType<PartyMember>();

        for (int i = 0; i < party.Count; i++)
        {
            party[i] = members[i];
            party[i].SetParty(this);
        }
    }

    /***COMMAND METHODS***/
    /// <summary>
    /// Called by multiple buttons to dictate the type of Order issued
    /// </summary>
    /// <param name="cmd">String command which convert to correct state</param>
    public void SetCommand(string cmd)
    {
        switch(cmd)
        {
            case "NONE": command = COMMAND.NONE; break;
            case "ATTACK":
                command = COMMAND.ATTACK;
                bm.SetState("SELECTION");
                break;
        }
    }

    /***MISCELLANEOUS***/
}
