using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Subclass of Party which contains the Player's characters
/// </summary>
public class PlayerParty : Party {

    /*** UI ***/

    /// <summary>
    /// The canvas containing all the buttons for issuing player Orders
    /// </summary>
    public GameObject commandsList;

    /// <summary>
    /// Text which appears when the player is choosing a target
    /// </summary>
    public Text targetingText;

    //The status of the player's actions; whether observing, issuing a command, selecting a target, etc.
    private enum STATE
    {
        IDLE, COMMANDS, SELECTION, PROJECTION, ENEMY_PROJECTION
    }
    private STATE state = STATE.IDLE;

    //Deactivates the command window on startup after calling base method
    protected override void Start()
    {
        commandsList.SetActive(false);
        targetingText.enabled = false;
    }

    //Monitors the player's actions in battle
    void Update()
    {

        //Clicking Left Mouse
        if (Input.GetMouseButtonDown(0))
        {
            switch(state)
            {
                //Activate Commands Menu
                case STATE.IDLE:
                    Entity e = getPartyMember();
                    if (e != null && e.Ready && !bm.isAnimating())
                    {
                        activeMember = e;
                        setCommandsDisplay(true); //Look at command UI
                        setPause(true); //Pause Game
                        state = STATE.COMMANDS;
                    }
                    break;

                //Select Target
                case STATE.SELECTION:
                    Entity t = getAnyMember();
                    if (t != null)
                    {
                        target = t;
                        activeMember.changeColor(Color.green);

                        calculateAction("ATTACK"); //Battle Projection calculation
                        state = STATE.PROJECTION;

                        executeAction();
                    }
                    break;

                case STATE.PROJECTION:
                    if (target.isHovering()) //Double click to perform action immediately
                    {
                        bm.okayButton();
                    }
                    break;

                default: break;
            }
        }

        //Other
        switch (state)
        {
            case STATE.COMMANDS:
                activeMember.changeColor(Color.green);
                break;

            case STATE.SELECTION:
                activeMember.changeColor(Color.green);
                break;

            case STATE.PROJECTION:
                activeMember.changeColor(Color.green);
                target.changeColor(Color.red);
                break;
        }

        //Hotkeys
        if (state != STATE.ENEMY_PROJECTION) cancellationKeyDown();
        okay();
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
            party[i].setParty(this);
        }
    }

    /***COMMAND METHODS***/
    /// <summary>
    /// Called by multiple buttons to dictate the type of Order issued
    /// </summary>
    /// <param name="cmd">String command which convert to correct state</param>
    public void setCommand(string cmd)
    {
        switch(cmd)
        {
            case "NONE": command = COMMAND.NONE; break;
            case "ATTACK":
                command = COMMAND.ATTACK;
                startSelection();
                break;
        }

        setCommandsDisplay(false);
    }

    /// <summary>
    /// Change state to Selection and choose a target - can be friendly or enemy
    /// </summary>
    public void startSelection()
    {
        state = STATE.SELECTION;
        targetingText.enabled = true;
    }

    /// <summary>
    /// Turn command window on or off
    /// </summary>
    /// <param name="b">true = on, false = off</param>
    public void setCommandsDisplay(bool b)
    {
        commandsList.SetActive(b);
    }

    /// <summary>
    /// Utilizes the Battle Manager's ability to pause/unpause the game
    /// </summary>
    /// <param name="b">True = pause game, false = unpuase game</param>
    public void setPause(bool b)
    {
        bm.setPause(b);
    }

    //Performs some kind of cancellation depnding on the state
    private void cancel()
    {
        switch (state)
        {
            case STATE.COMMANDS:
                setPause(false);
                resumeBattle();
                break;

            case STATE.SELECTION:
                setCommandsDisplay(true);
                state = STATE.COMMANDS;
                targetingText.enabled = false;
                break;

            case STATE.PROJECTION:
                bm.cancel();
                target.restoreColor();
                break;

            default: break;
        }
    }

    //Supplementary method to cancel; calls cancel after checking if any of the cancelling hotkeys are pressed
    private void cancellationKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetMouseButtonDown(1))
        {
            cancel();
        }
    }

    //Performs affirmative actions if any of the postive hotkeys are pressed
    private void okay()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            bm.okayButton();
        }
    }

    /// <summary>
    /// Deactive command window and cancel Order
    /// </summary>
    public void resumeBattle()
    {
        setCommandsDisplay(false); //Disable display
        targetingText.enabled = false;
        if (target != null) target.restoreColor();
        if (activeMember != null) activeMember.restoreColor();

        setCommand("NONE");
        activeMember = null; //Nullify any accidental actions with selected party member
        target = null;
        state = STATE.IDLE; //Set back to viewing mode
    }

    /***MISCELLANEOUS***/
    /// <summary>
    /// Return state such as IDLE, SELECTION, etc.
    /// </summary>
    /// <returns>state - the mode of play the player is currenlty in</returns>
    public string getState()
    {
        return state + "";
    }

    /// <summary>
    /// Changes state to ENEMY_PROJECTION, usually to bring up the enemy's Battle Projection window
    /// </summary>
    public void enemyMove()
    {
        state = STATE.ENEMY_PROJECTION;
        setPause(true);
    }
}
