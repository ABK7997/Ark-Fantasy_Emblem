using UnityEngine;
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

    private Inventory it; //items
    private int itemIndex = -1; //active item

    protected override void Start()
    {
        base.Start();

        it = FindObjectOfType<Inventory>();
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

                    /*
                    if (it.items[itemIndex].effect == Item.EFFECT.REVIVE
                        || activeMember.bc.activeSpecial.effect == Special.EFFECT.REVIVE
                        )
                    {
                        t = GetAnyMember();
                    }
                    else t = GetAnyLivingMember();
                    */
                    
                    if (t != null)
                    {
                        target = t;
                        activeMember.ChangeColor("active");

                        CalculateAction(); //Battle Projection calculation
                        bm.SetState("PLAYER_PROJECTION");

                        ExecuteAction();
                    }
                    break;
                
                //Select Tile
                case "TILE_SELECTION":

                    tileProspect = board.GetHoveringTile(activeMember, party, oppositeParty);

                    if (tileProspect != null)
                    {
                        activeMember.pc.SetTileProspect(tileProspect);

                        CalculateAction(); //Battle Projection calculation;
                        bm.SetState("PLAYER_PROJECTION");

                        ExecuteAction();
                    }

                    break;

                case "PLAYER_PROJECTION":
                    if (target == null) break;

                    else if (target.IsHovering()) //Double click to perform action immediately
                    {
                        activeMember.SetDefending(false);
                        bm.SetState("NORMAL");
                        StartCoroutine(bm.Animate());
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

            case "SELECTION": case "TILE_SELECTION": case "SPECIAL_SELECTION":
                activeMember.ChangeColor("active");
                break;

            case "PLAYER_PROJECTION":
                activeMember.ChangeColor("active");
                if (target != null) target.ChangeColor("target");
                break;
        }
    }

    /***COMMAND METHODS***/
    /// <summary>
    /// Called by multiple buttons in the commands list to dictate the type of Order issued
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

            case "ITEM":
                if (it.items.Count == 0) break; //Inventory is empty
                bm.SetState("SPECIAL_SELECTION");
                command = COMMAND.ITEM;
                ss.SetItems(it.items);
                break;

            case "MOVE":
                bm.SetState("TILE_SELECTION");
                command = COMMAND.MOVE;
                break;

            case "FLEE":
                command = COMMAND.FLEE;
                CalculateAction();
                ExecuteAction();
                bm.SetState("PLAYER_PROJECTION");
                break;
        }
    }

    /***BUTTONS***/

    /// <summary>
    /// Active party member is about to use a special ability
    /// </summary>
    public void SetSpecial(int specialIndex)
    {
        //Using an item, not a special
        if (command == COMMAND.ITEM)
        {
            if (it.items[specialIndex].stock == 0) return; //Item is out of stock
            else
            {
                bm.SetState("SELECTION");
                itemIndex = specialIndex;
            }

            return;
        }

        //Special
        switch (command)
        {
            case COMMAND.SKILL:
                if (activeMember.skills.Count <= specialIndex) return;
                break;

            case COMMAND.MAGIC:
                if (activeMember.spells.Count <= specialIndex) return;
                break;

            case COMMAND.TECH:
                if (activeMember.techs.Count <= specialIndex) return;
                break;

            default: break;
        }

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

        //Hit-All Special
        else if (activeMember.GetSpecial().hitAll)
        {
            if (activeMember.GetSpecial().type != Special.TYPE.ATTACK) target = activeMember;
            else target = oppositeParty[0];

            CalculateAction(); //Battle Projection calculation
            bm.SetState("PLAYER_PROJECTION");

            ExecuteAction();
        }

        //Other
        else bm.SetState("SELECTION");
    }

    /***MISCELLANEOUS***/

    /// <summary>
    /// Activate the command list with the first party member available
    /// </summary>
    public void GetFirstActive()
    {
        foreach (Entity e in party)
        {
            if (e.Ready)
            {
                activeMember = e;
                activeMember.SetSpecial(0, "NULL");
                bm.SetState("COMMANDING"); //Pause Game
            }
        }
    }

    /// <summary>
    /// Activate the command list with the party member associated with the given index
    /// </summary>
    /// <param name="index">party index</param>
    public void ActivateByIndex(int index)
    {
        if (index >= party.Count) return;
        else if (party[index].Hp == 0 || !party[index].Ready) return;

        activeMember = party[index];
        activeMember.SetSpecial(0, "NULL");
        bm.SetState("COMMANDING"); //Pause Game
    }

    /// <summary>
    /// Target an enemy by their index number
    /// </summary>
    /// <param name="index">party index</param>
    public void TargetEnemyByIndex(int index)
    {
        if (index >= oppositeParty.Count) return;

        target = oppositeParty[index];

        if (target.Hp == 0) return;

        activeMember.ChangeColor("active");

        CalculateAction(); //Battle Projection calculation
        bm.SetState("PLAYER_PROJECTION");

        ExecuteAction();
    }

    /// <summary>
    /// Target an ally by their index number
    /// </summary>
    /// <param name="index">party index</param>
    public void TargetAllyByIndex(int index)
    {
        if (index >= oppositeParty.Count) return;

        target = party[index];

        if (target.Hp == 0) return;

        activeMember.ChangeColor("active");

        CalculateAction(); //Battle Projection calculation
        bm.SetState("PLAYER_PROJECTION");

        ExecuteAction();
    }

    /// <summary>
    /// The current item to consume
    /// </summary>
    /// <returns>A consumable item</returns>
    public Item GetItem()
    {
        return it.items[itemIndex];
    }

}
