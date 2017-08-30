using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A superclass containing everything similar to Player and Enemy parties
/// </summary>
public abstract class Party : MonoBehaviour {

    protected BattleManager bm; //Set in the BattleManager class during battle setup

    /// <summary>The character which represents the entire party outside of battle in the overworld </summary>
    public GameObject avatar;

    /// <summary>List of playable party members</summary>
    public List<Entity> party; //

    /// <summary>List of enemies being fought</summary>
    [HideInInspector] public List<Entity> oppositeParty;

    protected Entity activeMember, target; //The acting member and its target respectively

    //Organizes the party on startup
    protected virtual void Start () {
	}

    //The Different moves types an entity can perform
    protected enum COMMAND
    {
        NONE, ATTACK
    }
    protected COMMAND command = COMMAND.NONE;

    /// <summary>
    /// Loads the party properly with either partyMembers or Enemies (defined in subclasses)
    /// </summary>
    public abstract void OrganizeParty();

    //Party Management
    public void add(Entity e)
    {
        party.Add(e);
    }
    public void remove(Entity e)
    {
        party.Remove(e);
    }

    /// <summary>
    /// Assigns the opposing party list
    /// </summary>
    /// <param name="otherParty"></param>
    public void constructOppositeParty(List<Entity> otherParty)
    {
        oppositeParty = otherParty;
    }

    //Iterates through every member to find if any of them are being hovered over
    protected Entity getPartyMember()
    {
        foreach (Entity e in party)
        {
            if (e.isHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        return null;
    }

    //Iterates through every enemy to find if any of them are being hovered over
    protected Entity getOppositeMember()
    {
        foreach (Entity e in oppositeParty)
        {
            if (e.isHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        return null;
    }

    //Iterates through every member of BOTH parties to find if any of them are being hovered over
    protected Entity getAnyMember()
    {
        foreach (Entity e in party)
        {
            if (e.isHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        foreach (Entity e in oppositeParty)
        {
            if (e.isHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        return null;
    }

    /// <summary>
    /// Used by the BattleManager class to assign itself to the parties bm variable
    /// </summary>
    /// <param name="manager"></param>
    public void setBattleManager(BattleManager manager)
    {
        bm = manager;
    }

    /// <summary>
    /// Adds a new Order to the BattleManager class queue 
    /// </summary>
    public void executeAction()
    {
        bm.issueOrder(command + "", activeMember, target);
    }

    /// <summary>
    /// Issue and order to the action queue
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="user"></param>
    /// <param name="trgt"></param>
    public void executeAction(string cmd, Entity user, Entity trgt)
    {
        bm.issueOrder(cmd, user, trgt);
        target = trgt;
        activeMember = user;
    }

    /***BATTLE CALCULATIONS***/

    /// <summary>
    /// Calculate information needed for the BattleManger's battle calculation window (Atk, hit, critical)
    /// </summary>
    /// <param name="type"> the kind of action to be performed, such as ATTACK or MAGIC </param>
    protected void calculateAction(string type)
    {
        switch (type)
        {
            case "ATTACK": bm.setProjectionInfo(activeMember.atk, 100, 0); break;
            default: break;
        }
    }
}
