using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// A superclass containing everything similar to Player and Enemy parties
/// </summary>
public abstract class Party : MonoBehaviour {

    protected BattleManager bm; //Set in the BattleManager class during battle setup
    protected BattleUI ui; //Set in the Battlemanager

    /// <summary>List of playable party members</summary>
    public List<Entity> party; //For Enemies or PartyMembers

    /// <summary>List of enemies being fought</summary>
    [HideInInspector] public List<Entity> oppositeParty; //The others

    protected Entity activeMember, target; //The acting member and its target respectively

    //Organizes the party on startup
    protected virtual void Start () {
	}

    protected virtual void Update()
    {
        if (bm.GetState() == "NORMAL")
        {
            foreach (Entity e in party)
            {
                e.UpdateTime();
            }
        }
    }

    //The Different moves types an entity can perform
    protected enum COMMAND
    {
        NONE, ATTACK, SKILL, MAGIC, TECH, DEFEND, FLEE
    }
    protected COMMAND command = COMMAND.NONE;

    /// <summary>
    /// Loads the party properly with either partyMembers or Enemies (defined in subclasses)
    /// </summary>
    public void OrganizeParty(Vector2[] coords, int scaling, List<Entity> newParty)
    {
        foreach (Entity e in newParty)
        {
            party.Add(e);
        }

        for (int i = 0; i < coords.Length; i++)
        {
            if (i == party.Count) break;

            Entity e = Instantiate(party[i], new Vector2(coords[i].x * scaling, coords[i].y * scaling), Quaternion.identity, transform);
            party[i] = e;
            party[i].SetParty(this);
        }
    }

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
            if (e.IsHovering() && e.GetStatus() != "DEAD") return e;
        }

        return null;
    }

    //Iterates through every enemy to find if any of them are being hovered over
    protected Entity GetOppositeMember()
    {
        foreach (Entity e in oppositeParty)
        {
            if (e.IsHovering() && e.GetStatus() != "DEAD") return e;
        }

        return null;
    }

    //Iterates through every member of BOTH parties to find if any of them are being hovered over
    protected Entity GetAnyMember()
    {
        foreach (Entity e in party)
        {
            if (e.IsHovering() && e.GetStatus() != "DEAD") return e;
        }

        foreach (Entity e in oppositeParty)
        {
            if (e.IsHovering() && e.GetStatus() != "DEAD") return e;
        }

        return null;
    }

    /// <summary>
    /// Used by the BattleManager class to assign itself to the parties bm variable
    /// </summary>
    /// <param name="manager"></param>
    public void SetBattleManager(BattleManager manager, BattleUI bui)
    {
        bm = manager;
        ui = bui;
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
    protected void CalculateAction()
    {
        activeMember.SetTemporaryStats(target);

        switch (command)
        {
            case COMMAND.ATTACK:
                ui.SetProjectionInfo(activeMember.PhysicalDmg, activeMember.Hit, activeMember.Crit);
                break;

            case COMMAND.MAGIC:
                switch (activeMember.GetSpecial().type)
                {
                    case Special.TYPE.ATTACK:
                        ui.SetProjectionInfo(activeMember.MagicDmg, activeMember.Hit, activeMember.Crit); break;

                    case Special.TYPE.HEAL:
                        ui.SetProjectionInfo(-activeMember.MagicDmg, activeMember.Crit); break;
                }
                
                break;

            case COMMAND.TECH:
                switch (activeMember.GetSpecial().type)
                {
                    case Special.TYPE.ATTACK:
                        ui.SetProjectionInfo(activeMember.TechDmg, activeMember.Hit, activeMember.Crit); break;

                    case Special.TYPE.REPAIR:
                        ui.SetProjectionInfo(-activeMember.TechDmg, activeMember.Crit); break;
                }

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
        if (activeMember != null)
        {
            activeMember.ChangeColor("normal");
            activeMember.SetSpecial(-1, null);
        }
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

    /// <summary>
    /// Set the currently-set special ability to null so it can't be used by accident
    /// </summary>
    public void NullifySpecial()
    {
        activeMember.SetSpecial(-1, null);
    }

    public void Normalize()
    {
        bm.SetState("NORMAL");
    }

    /***GET PARTY***/

    /// <summary>
    /// Sends the entire party to another class
    /// </summary>
    /// <returns>party - this particular party instance (either player or enemy)</returns>
    public List<Entity> GetParty()
    {
        return party;
    }

    /// <summary>
    /// Only get the party members still alive
    /// </summary>
    /// <returns>retParty - the members with HP above 0</returns>
    public List<Entity> GetLivingParty()
    {
        List<Entity> retParty = party;

        foreach (Entity e in retParty)
        {
            if (e.GetStatus() == "DEAD")
            {
                retParty.Remove(e);
            }
        }

        return retParty;
    }

    /// <summary>
    /// Get all members of the opposite party who are still alive
    /// </summary>
    /// <returns></returns>
    public List<Entity> GetLivingEnemies()
    {
        List<Entity> enemies = oppositeParty;

        foreach (Entity e in enemies)
        {
            if (e.GetStatus() == "DEAD") enemies.Remove(e);
        }

        return enemies;
    }

    /// <summary>
    /// Get the currently-acting party member
    /// </summary>
    /// <returns>The party member in action</returns>
    public Entity GetActiveMember()
    {
        return activeMember;
    }
}
