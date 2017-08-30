using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Subclass of Party which contains the Player's characters
/// </summary>
public class PlayerParty : Party {

    /*** UI ***/

    //Deactivates the command window on startup after calling base method
    protected override void Start()
    {
        
    }

    //Monitors the player's actions in battle
    void Update()
    {

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
                        activeMember.ChangeColor(Color.green);

                        CalculateAction("ATTACK"); //Battle Projection calculation
                        bm.SetState("PLAYER_PROJECTION");

                        ExecuteAction();
                    }
                    break;

                case "PLAYER_PROJECTION":
                    if (target.IsHovering()) //Double click to perform action immediately
                    {
                        bm.OkayButton();
                    }
                    break;

                default: break;
            }
        }

        //Other
        switch (bm.GetState())
        {
            case "COMMANDING":
                activeMember.ChangeColor(Color.green);
                break;

            case "SELECTION":
                activeMember.ChangeColor(Color.green);
                break;

            case "PLAYER_PROJECTION":
                activeMember.ChangeColor(Color.green);
                target.ChangeColor(Color.red);
                break;
        }

        //Hotkeys
        if (bm.GetState() != "ENEMY_PROJECTION") CancellationKeyDown();
        Okay();
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

    //Performs some kind of cancellation depnding on the state
    private void Cancel()
    {
        switch (bm.GetState())
        {
            case "COMMANDS":
                bm.SetState("NORMAL");
                ResumeBattle();
                break;

            case "SELECTION":
                bm.SetState("COMMANDS");
                break;

            case "PLAYER_PROJECTION":
                bm.SetState("COMMANDING");
                bm.Cancel();
                target.RestoreColor();
                break;

            default: break;
        }
    }

    //Supplementary method to cancel; calls cancel after checking if any of the cancelling hotkeys are pressed
    private void CancellationKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetMouseButtonDown(1))
        {
            Cancel();
        }
    }

    //Performs affirmative actions if any of the postive hotkeys are pressed
    private void Okay()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            bm.OkayButton();
        }
    }

    /// <summary>
    /// Deactive command window and cancel Order
    /// </summary>
    public void ResumeBattle()
    {
        if (target != null) target.RestoreColor();
        if (activeMember != null) activeMember.RestoreColor();

        SetCommand("NONE");
        activeMember = null; //Nullify any accidental actions with selected party member
        target = null;
        bm.SetState("NORMAL"); //Set back to viewing mode
    }

    /***MISCELLANEOUS***/

    /// <summary>
    /// Changes state to ENEMY_PROJECTION, usually to bring up the enemy's Battle Projection window
    /// </summary>
    public void EnemyMove()
    {
        bm.SetState("ENEMY_PROJECTION");
    }
}
