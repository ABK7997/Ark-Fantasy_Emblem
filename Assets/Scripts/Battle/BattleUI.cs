using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public static bool moving = false;
    public static bool fleeSuccess = false;

    /// <summary>The Battle Manager, mostly just for its GetState() method</summary>
    public BattleManager bm;

    /// <summary>The canvas containing all the buttons for issuing player Orders</summary>
    public GameObject commandsList;

    /// <summary>
    /// The text associated with the Tech button on the command list. 
    /// If a Tech timer is above 0, that number is displayed in red on the button.
    /// </summary>
    public Text techButtonText;

    /// <summary>
    /// The window containing the active party member's special abilties for selection
    /// </summary>
    public Canvas specialSelection;

    ///<summary>An overlaying, half-transparent image that visually declares to the player when the game is paused or not </summary>
    public Canvas pauseScreen;

    /// <summary>
    /// The button which manually pauses the action. Unavailabe in Battle Prep state
    /// </summary>
    public Button pauseButton;

    /// <summary>Text which appears when the player is choosing a target</summary>
    public Text targetingText;

    /// <summary>On-screen button to cancel targetting if the player does not prefer using hotkeys</summary>
    public Button targetCancelButton;

    /// <summary>
    /// Window which pops up when a character gains enough experience to level up after a move
    /// </summary>
    public Image levelUpWindow;

    /// <summary>
    /// Text to display in the level up window when a character is leveled up
    /// </summary>
    public Text levelUpText;

    /// <summary>
    /// Window to display after all enemies are dead
    /// </summary>
    public Image victoryWindow;

    /// <summary>
    /// Window to display after all party members are dead
    /// </summary>
    public Image deathWindow;

    /// <summary>
    /// Window which reports on if a flee attempt was successful or not
    /// </summary>
    public Image fleeReport;

    /// <summary>
    /// Text saying if fleeing was successful or not
    /// </summary>
    public Text fleeText;

    /***BATTLE PROJECTION***/

    /// <summary>
    /// A box which is used to display the battle projection stats
    /// </summary>
    public Image battleProjection;

    /// <summary>
    /// Text within the Battle Projection canvas regarding the initiator
    /// </summary>
    public Text projectionInfo;

    /// <summary>
    /// A box which displays battle projection stats for enemy moves
    /// </summary>
    public Image enemyProjection;

    /// <summary>
    /// Text within the enemy Battle Projection canvas regarding the action
    /// </summary>
    public Text eProjectionInfo;

    /***BUTTONS***/

    /// <summary> The cancel button in the BP window; unavailable if it is an Enemy Projection</summary>
    public Button cancelButton;

    /***STATES***/

    /// <summary>
    /// Determines which UI elements to turn on or off depending on the game state
    /// </summary>
    public void ChangingState()
    {
        switch (bm.GetState())
        {
            case "NORMAL":
                pauseScreen.gameObject.SetActive(false);
                commandsList.SetActive(false);
                SetProjection(false, "");
                SetEnemyProjection(false, "");
                cancelButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(true);
                levelUpWindow.gameObject.SetActive(false);
                fleeReport.gameObject.SetActive(false);
                break;

            case "ANIMATING":
                
                break;

            case "COMMANDING":
                SetTargetting(false);

                commandsList.SetActive(true);
                specialSelection.enabled = false;
                pauseButton.gameObject.SetActive(false);

                int timer = bm.pParty.GetActiveMember().TechTimer;

                if (timer == 0)
                {
                    techButtonText.text = "5) TECH";
                    techButtonText.color = Color.black;
                }
                else if (timer == 1)
                {
                    techButtonText.text = "5) TECH recharging: " + timer + " turn";
                    techButtonText.color = Color.gray;
                }
                else
                {
                    techButtonText.text = "5) TECH recharging: " + timer + " turns";
                    techButtonText.color = Color.gray;
                }

                break;

            case "SPECIAL_SELECTION":
                commandsList.SetActive(false);
                SetTargetting(false);
                SetProjection(false, "");
                specialSelection.enabled = true;
                break;

            case "TILE_SELECTION":
                commandsList.SetActive(false);
                SetProjection(false, "");
                break;

            case "SELECTION":
                commandsList.SetActive(false);
                SetProjection(false, "");

                SetTargetting(true);
                specialSelection.enabled = false;
                break;

            case "LEVEL_UP":
                levelUpWindow.gameObject.SetActive(true);
                break;

            case "PLAYER_PROJECTION":
                SetTargetting(false);
                SetTargetting(false);
                specialSelection.enabled = false;
                commandsList.SetActive(false);
                break;

            case "ENEMY_PROJECTION":
                cancelButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(false);
                break;

            case "PAUSED":
                pauseScreen.gameObject.SetActive(true);
                break;

            case "VICTORY":
                victoryWindow.gameObject.SetActive(true);
                break;

            case "GAME_OVER":
                deathWindow.gameObject.SetActive(true);
                break;

            case "FLEE_REPORT":
                fleeReport.gameObject.SetActive(true);

                string text = "";

                if (fleeSuccess) text = "Escaped from battle";
                else text = "Could not escape!";

                fleeText.text = text;

                break;

            default: break;
        }
    }

    /// <summary>
    /// Enable battle projection according to move type
    /// </summary>
    /// <param name="isPlayer">If the entity making a move is the player or not</param>
    /// <param name="special">The active special, if one is being used (can be null)</param>
    /// <param name="user">The entity making the move</param>
    public void SetProjectionInfo(bool isPlayer, string command, Entity user)
    {
        Special special = user.GetSpecial();

        string text = "";

        //Physical Attack
        if (command == "ATTACK") text = SingleTargetProjection(user, command);

        //Special - Spell, Tech, or Skill
        else if (special != null)
        {
            //Multi-target
            if (special.hitAll && special.type == Special.TYPE.ATTACK) text = AttackAllProjection(user, command);

            //Healing (single and multiple)
            else if (special.type == Special.TYPE.HEAL || special.type == Special.TYPE.REPAIR)
            {
                text = HealProjection(user, command);
            }

            //Effects and Skills
            else if (special.type == Special.TYPE.EFFECT) text = EffectProjection(user);

            //Single-target Offensive
            else text = SingleTargetProjection(user, command);
        }

        //Flee
        else if (command == "FLEE")
        {
            text = "Flee Chance: " + user.bc.fleeChance + "\n\n" +
                "Run away from the battle. Strong monsters or large enemy parties are more difficult to flee from.";
        }

        //Move
        else
        {
            text = MoveProjection(user);
            moving = true;
        }

        //Enable Projection Window UI
        if (isPlayer) SetProjection(true, text);
        else SetEnemyProjection(true, text);
    }

    //Single-Target Offensive Attack
    private string SingleTargetProjection(Entity user, string command)
    {
        BattleCalculator bc = user.bc;

        string ret = "";
        string description;

        //Set Description
        if (user.GetSpecial() != null) description = user.GetSpecial().description;
        else description = "Physical Attack"; 

        //Set Power
        int damage = 0;

        switch (command)
        {
            case "ATTACK": damage = bc.PhysicalDmg; break;
            case "MAGIC": damage = bc.MagicDmg; break;
            case "TECH": damage = bc.TechDmg; break;
        }

        ret =
            "PWR: " + damage + "\n" +
            "HIT: " + bc.Hit + "%\n" +
            "CRIT: " + bc.Crit + "%" + "\n\n" +
            description;

        return ret;
    }

    //Multi-Target Offensive Attack
    private string AttackAllProjection(Entity user, string command)
    {
        BattleCalculator bc = user.bc;

        string ret = "";

        //Set Power
        int damage = 0;

        switch (command)
        {
            case "MAGIC": damage = (int)(bc.MagicDmg * user.GetSpecial().basePwr); break;
            case "TECH": damage = (int)(bc.TechDmg * user.GetSpecial().basePwr); break;
        }

        ret =
            "BASE PWR: ~" + damage + "\n\n" +
            "Damage and hit chance will vary from target to target";

        return ret;
    }

    //Multi-Target Healing/Repairing
    private string HealProjection(Entity user, string command)
    {
        BattleCalculator bc = user.bc;

        string ret = "";
        int damage = 0;

        switch (command)
        {
            case "MAGIC": damage = -bc.MagicDmg; break;
            case "TECH": damage = -bc.TechDmg; break;
        }

        ret =
            "PWR: " + damage + "\n" +
            "BONUS: " + bc.Crit + "%" + "\n\n" +
            user.GetSpecial().description;

        return ret;
    }

    //Effect
    private string EffectProjection(Entity user)
    {
        string ret = "";

        ret =
            user.GetSpecial().effect + "\n" +
            "Success: " + user.bc.Hit + "%" + "\n\n" +
            user.GetSpecial().description;

        return ret;
    }

    //Move to Tile
    private string MoveProjection(Entity user)
    {
        string ret;
        Tile prospect = user.pc.tileProspect;

        ret =
            "Move To " + prospect.tileName + "\n\nTerrain Effects:";

        if (prospect.effect1 != Tile.EFFECT.NONE) ret += "\n" + prospect.effect1;
        if (prospect.effect2 != Tile.EFFECT.NONE) ret += "\n" + prospect.effect2;

        return ret;
    }

    /// <summary>
    /// Enable Level Up window and change text approriately
    /// </summary>
    /// <param name="text">The stats of the entity leveling up</param>
    public void SetLevelUpText(string text)
    {
        bm.SetState("LEVEL_UP");
        levelUpText.text = text;
    }

    //Shorthand to enabling/disabling the Battle Projection game object
    private void SetProjection(bool b, string text)
    {
        battleProjection.gameObject.SetActive(b);
        projectionInfo.text = text;
    }

    //Shorthand to enabling.disabling the Enemy's BP game object
    private void SetEnemyProjection(bool b, string text)
    {
        enemyProjection.gameObject.SetActive(b);
        eProjectionInfo.text = text;
    }

    //Shorthand to enable/disable targetting UI elements
    private void SetTargetting(bool b)
    {
        targetingText.enabled = b;
        targetCancelButton.gameObject.SetActive(b);
    }
}
