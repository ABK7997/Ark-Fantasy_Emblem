using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// A superclass containing everything similar to Player and Enemy parties
/// </summary>
public abstract class Party : MonoBehaviour {

    public Image statsView;
    public Text statsText;

    protected bool isPlayer;

    protected BattleManager bm; //Set in the BattleManager class during battle setup
    protected BattleUI ui; //Set in the Battlemanager
    protected BoardManager board;

    /// <summary>List of playable party members</summary>
    public List<Entity> party; //For Enemies or PartyMembers

    /// <summary>List of enemies being fought</summary>
    [HideInInspector] public List<Entity> oppositeParty; //The others
    [HideInInspector] public Party otherPartyBody;

    protected Entity activeMember, target; //The acting member and its target respectively
    protected Tile tileProspect; //Tile the active member may move to

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
        NONE, ATTACK, SKILL, MAGIC, TECH, DEFEND, MOVE, FLEE
    }
    protected COMMAND command = COMMAND.NONE;

    /// <summary>
    /// Loads the party properly with either partyMembers or Enemies (defined in subclasses)
    /// </summary>
    public void OrganizeParty(bool isPlayer, Vector2[] coords, int scaling, List<Entity> newParty)
    {
        this.isPlayer = isPlayer;

        foreach (Entity e in newParty)
        {
            party.Add(e);
        }

        for (int i = 0; i < coords.Length; i++)
        {
            if (i == party.Count) break;

            //Party Member
            Entity e = Instantiate(party[i], new Vector2(coords[i].x * scaling, coords[i].y * scaling), Quaternion.identity, transform);
            party[i] = e;
            party[i].Index = i;
            party[i].indexText.text = (i + 1) + "";
            party[i].SetParty(this);
            party[i].pc.SetOriginalPosition(coords[i].x * scaling, coords[i].y * scaling);

            //Tile member is instantiated onto
            Tile t = board.GetTile(party[i].transform.position);
            party[i].pc.SetTile(t);
            //t.Occupied = true;
        }
    }

    //Party Management

    /// <summary>
    /// Add a member to this party
    /// </summary>
    /// <param name="e">Entity to be added </param>
    public void Add(Entity e)
    {
        party.Add(e);
    }

    /// <summary>
    /// Remove a member from this party
    /// </summary>
    /// <param name="e">Entity to be removed</param>
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
    public void SetBattleManager(BattleManager manager, BattleUI bui, BoardManager brd)
    {
        bm = manager;
        ui = bui;
        board = brd;
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
        activeMember.bc.SetTemporaryStats(target);

        string info = "";

        if (activeMember.GetSpecial() != null) info = activeMember.GetSpecial().description;

        switch (command)
        {
            case COMMAND.ATTACK:
                ui.SetProjectionInfo(isPlayer, activeMember.bc.PhysicalDmg, activeMember.bc.Hit, activeMember.bc.Crit, "Physical Attack");
                break;

            case COMMAND.MAGIC:
                switch (activeMember.GetSpecial().type)
                {
                    case Special.TYPE.ATTACK:
                        ui.SetProjectionInfo(isPlayer, activeMember.bc.MagicDmg, activeMember.bc.Hit, activeMember.bc.Crit, info); break;

                    case Special.TYPE.HEAL:
                        ui.SetProjectionInfo(isPlayer, -activeMember.bc.MagicDmg, activeMember.bc.Crit, info); break;
                }
                
                break;

            case COMMAND.TECH:
                switch (activeMember.GetSpecial().type)
                {
                    case Special.TYPE.ATTACK:
                        ui.SetProjectionInfo(isPlayer, activeMember.bc.TechDmg, activeMember.bc.Hit, activeMember.bc.Crit, info); break;

                    case Special.TYPE.REPAIR:
                        ui.SetProjectionInfo(isPlayer, - activeMember.bc.TechDmg, activeMember.bc.Crit, info); break;
                }

                break;

            case COMMAND.SKILL:
                switch (activeMember.GetSpecial().type)
                {
                    case Special.TYPE.EFFECT:
                        ui.SetProjectionInfo(isPlayer, activeMember.GetSpecial().effect + "", activeMember.bc.Hit, info); break;
                }

                break;

            case COMMAND.MOVE:
                ui.SetProjectionInfo(isPlayer, tileProspect);
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
            //activeMember.ResetPosition();
        }
        activeMember = null;

        if (target != null)
        {
            target.ChangeColor("normal");
            //target.ResetPosition();
        }
        target = null;
    }

    /// <summary>
    /// Reset every party member to their regular position
    /// </summary>
    public void ResetPositionAll()
    {
        foreach (Entity e in party)
        {
            e.pc.ResetPosition();
        }
    }

    /// <summary>
    /// Adjust index display when a party member dies or is revived
    /// </summary>
    public void UpdateIndeces()
    {
        int i = 0;
        foreach (Entity e in party)
        {
            if (e.Hp > 0)
            {
                e.Index = i;
                e.indexText.text = "" + (i + 1);
                i++;
            }
            else e.indexText.text = "";
        }
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

    /// <summary>
    /// Return the battle to a NORMAL game state
    /// </summary>
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
        List<Entity> deadParty = new List<Entity>();

        foreach (Entity e in retParty)
        {
            if (e.GetStatus() == "DEAD")
            {
                deadParty.Add(e);
            }
        }

        foreach (Entity e in deadParty)
        {
            retParty.Remove(e);
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
        List<Entity> deadEnemies = new List<Entity>();

        foreach (Entity e in enemies)
        {
            if (e.GetStatus() == "DEAD") deadEnemies.Add(e);
        }

        foreach (Entity e in deadEnemies)
        {
            enemies.Remove(e);
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

    /// <summary>
    /// Move a party member to the prospective tile
    /// </summary>
    public void MoveMember()
    {
        activeMember.pc.SetTile(tileProspect);
        activeMember.pc.SetPosition(tileProspect.transform.position.x, tileProspect.transform.position.y);
    }

    /***LEVELING***/
    /// <summary>
    /// Activate level up window and change text for active member
    /// </summary>
    public void SetLevelUpText(bool[] up, Entity e)
    {
        string text = e.Name + "\n";
        text += "Leveled Up\n";

        if (up[0]) text += "\nHP: " + (e.Hp-1) + " + 1";

        //OFFENSIVE
        if (up[1]) text += "\nATK: " + (e.Atk - 1) + " + 1";
        if (up[2]) text += "\nMAG: " + (e.Mag - 1) + " + 1";
        if (up[3]) text += "\nVLT: " + (e.Vlt - 1) + " + 1";

        //DEFENSIVE
        if (up[4]) text += "\nDEF: " + (e.Def - 1) + " + 1";
        if (up[5]) text += "\nRES: " + (e.Res - 1) + " + 1";
        if (up[6]) text += "\nSTB: " + (e.Stb - 1) + " + 1";

        //PERFORMANCE
        if (up[7]) text += "\nSKL: " + (e.Skl - 1) + " + 1";
        if (up[8]) text += "\nLCK: " + (e.Lck - 1) + " + 1";
        if (up[9]) text += "\nSPD: " + (e.Spd - 1) + " + 1";

        ui.SetLevelUpText(text);
    }

    public void SetStatsView(bool b, string text)
    {
        statsView.gameObject.SetActive(b);
        statsText.text = text;
    }
}
