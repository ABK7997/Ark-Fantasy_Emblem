﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Subclass of Party which contains the Player's characters
/// </summary>
public class PlayerParty : Party {

    /// <summary>
    /// The selection window of the battle screen
    /// </summary>
    public SpecialSelection ss;

    protected override void Start()
    {
        base.Start();
    }

    //Monitors the player's actions in battle
    protected override void Update()
    {
        if (bm == null) return; //Battle Manager not set

        base.Update();

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
                        activeMember.SetSpecial(0, "NULL");
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

                        CalculateAction(); //Battle Projection calculation
                        bm.SetState("PLAYER_PROJECTION");

                        ExecuteAction();
                    }
                    break;

                case "PLAYER_PROJECTION":
                    if (target.IsHovering()) //Double click to perform action immediately
                    {
                        activeMember.SetDefending(false);
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

            case "DEFEND":
                command = COMMAND.DEFEND;
                activeMember.SetDefending(true);
                bm.SetState("NORMAL");
                break;

            case "MAGIC":
                if (activeMember.spells.Count == 0) break; //Non-magical PC
                bm.SetState("SPECIAL_SELECTION");
                command = COMMAND.MAGIC;
                ss.SetSpecials(activeMember.spells);
                break;

            case "TECH":
                if (activeMember.techs.Count == 0 || activeMember.TechTimer > 0) break; //PC is not a Droid

                bm.SetState("SPECIAL_SELECTION");
                command = COMMAND.TECH;
                ss.SetSpecials(activeMember.techs);

                break;

            case "SKILL":
                if (activeMember.skills.Count == 0) break; //Unskilled PC
                bm.SetState("SPECIAL_SELECTION");
                command = COMMAND.SKILL;
                ss.SetSpecials(activeMember.skills);
                break;
        }
    }

    /***BUTTONS***/

    /// <summary>
    /// Active party member is about to use a special ability
    /// </summary>
    public void Special(int specialIndex)
    {
        activeMember.SetSpecial(specialIndex, command + "");

        //Skill selected
        if (command == COMMAND.SKILL)
        {
            target = activeMember;
            activeMember.ChangeColor("target");

            CalculateAction(); //Battle Projection calculation
            bm.SetState("PLAYER_PROJECTION");

            ExecuteAction();
        }

        else bm.SetState("SELECTION");
    }

    /***MISCELLANEOUS***/
}
