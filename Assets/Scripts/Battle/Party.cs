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
    public void Add(Entity e)
    {
        party.Add(e);
    }
    public void Remove(Entity e)
    {
        party.Remove(e);
    }

    /// <summary>
    /// Assigns the opposing party list
    /// </summary>
    /// <param name="otherParty"></param>
    public void ConstructOppositeParty(List<Entity> otherParty)
    {
        oppositeParty = otherParty;
    }

    //Iterates through every member to find if any of them are being hovered over
    protected Entity GetPartyMember()
    {
        foreach (Entity e in party)
        {
            if (e.IsHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        return null;
    }

    //Iterates through every enemy to find if any of them are being hovered over
    protected Entity GetOppositeMember()
    {
        foreach (Entity e in oppositeParty)
        {
            if (e.IsHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        return null;
    }

    //Iterates through every member of BOTH parties to find if any of them are being hovered over
    protected Entity GetAnyMember()
    {
        foreach (Entity e in party)
        {
            if (e.IsHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        foreach (Entity e in oppositeParty)
        {
            if (e.IsHovering() && e.Status != Entity.STATUS.DEAD) return e;
        }

        return null;
    }

    /// <summary>
    /// Used by the BattleManager class to assign itself to the parties bm variable
    /// </summary>
    /// <param name="manager"></param>
    public void SetBattleManager(BattleManager manager)
    {
        bm = manager;
    }

    /// <summary>
    /// Adds a new Order to the BattleManager class queue 
    /// </summary>
    public void ExecuteAction()
    {
        bm.IssueOrder(command + "", activeMember, target);
    }

    /// <summary>
    /// Issue and order to the action queue
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="user"></param>
    /// <param name="trgt"></param>
    public void ExecuteAction(string cmd, Entity user, Entity trgt)
    {
        bm.IssueOrder(cmd, user, trgt);
        target = trgt;
        activeMember = user;
    }

    /***BATTLE CALCULATIONS***/

    /// <summary>
    /// Calculate information needed for the BattleManger's battle calculation window (Atk, hit, critical)
    /// </summary>
    /// <param name="type"> the kind of action to be performed, such as ATTACK or MAGIC </param>
    protected void CalculateAction(string type)
    {
        switch (type)
        {
            case "ATTACK":
                bm.SetProjectionInfo(activeMember.Atk, 100, 0);
                break;
            default: break;
        }
    }

    /***COMMANDS***/

    /// <summary>
    /// Quick way to nullify and restore color to the target and active entities
    /// </summary>
    public void ResetState()
    {
        if (activeMember != null) activeMember.ChangeColor("normal");
        activeMember = null;

        if (target != null) target.ChangeColor("normal");
        target = null;
    }

    /// <summary>
    /// Similar to ResetState(), but only used to nullify and remove color from a target entity
    /// </summary>
    public void CancelTarget()
    {
        if (target != null) target.ChangeColor("normal");
        target = null;
    }

}
