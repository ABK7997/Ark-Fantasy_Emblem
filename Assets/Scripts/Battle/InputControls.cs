using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class dedicated to how the player interfaces with the game through the keyboard and mouse
/// </summary>
public class InputControls : MonoBehaviour {

    /// <summary>
    /// Battle Manager for the combat system
    /// </summary>
    public BattleManager bm;

    /// <summary>
    /// The player's party of characters
    /// </summary>
    public PlayerParty pParty;
	
	// Update is called once per frame
	void Update () {
        CancellationKeys();
        ConfirmationKeys();
        PauseKeys();

        switch (bm.GetState())
        {
            case "NORMAL":
                PartySelectHotkeys();
                return;

            case "COMMANDING":
                CommandHotkeys();
                return;
        }
	}

    //Cancellation hotkeys
    private void CancellationKeys()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetMouseButtonDown(1) ||
            Input.GetKeyDown(KeyCode.C) ||
            Input.GetKeyDown(KeyCode.Keypad0)
            )
        {
            Cancel();
        }
    }

    /// <summary>
    /// Method which can be used by UI buttons and typically sets the battle back one state (i.e. SELECTION to COMMANDs)
    /// </summary>
    public void Cancel()
    {
        string type = "";

        if (pParty.GetActiveMember().GetSpecial() != null)
        {
            type = pParty.GetActiveMember().GetSpecial().type + "";
        }

        switch (bm.GetState())
        {
            case "COMMANDING":
            case "PAUSED":
                bm.SetState("NORMAL");
                break;

            case "SELECTION":
                pParty.NullifySpecial();
                if (type != "")
                {
                    bm.SetState("SPECIAL_SELECTION");
                }
                else bm.SetState("COMMANDING");
                break;

            case "SPECIAL_SELECTION":
                pParty.NullifySpecial();
                bm.SetState("COMMANDING");
                break;

            case "PLAYER_PROJECTION":
                bm.CancelAction();
                pParty.CancelTarget();

                if (type == "EFFECT") bm.SetState("SPECIAL_SELECTION");
                else bm.SetState("SELECTION");

                break;
        }
    }

    //OK or affirmative hotkeys
    private void ConfirmationKeys()
    {
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.V)
            )
        {
            Confirm();
        }
    }

    /// <summary>
    /// Method which can be used by UI buttons to do the equivalent of any confirming or "OK" action
    /// </summary>
    public void Confirm()
    {
        switch (bm.GetState())
        {
            case "NORMAL":
                pParty.GetFirstActive();
                break;

            case "PLAYER_PROJECTION":
            case "ENEMY_PROJECTION":
                bm.SetState("NORMAL");
                break;
        }
    }

    //Pausing hotkeys
    private void PauseKeys()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TogglePause();
        }
    }

    /// <summary>
    /// Method which can be used by UI buttons to manually pause/unpause the game
    /// </summary>
    public void TogglePause()
    {
        if (bm.GetState() == "NORMAL") bm.SetState("PAUSED");
        else if (bm.GetState() == "PAUSED") bm.SetState("NORMAL");
    }

    //Player Selection Hotkeys
    private void PartySelectHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) pParty.ActivateByIndex(0);
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) pParty.ActivateByIndex(1);
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) pParty.ActivateByIndex(2);
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) pParty.ActivateByIndex(3);
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) pParty.ActivateByIndex(4);
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) pParty.ActivateByIndex(5);
    }

    //Command Hotkeys
    private void CommandHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) pParty.SetCommand("ATTACK"); //1. ATTACK
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) pParty.SetCommand("DEFEND"); //2. DEFEND
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) pParty.SetCommand("SKILL"); //3. SKILLS
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) pParty.SetCommand("MAGIC"); //4. MAGIC
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) pParty.SetCommand("TECH"); //5. TECHS
    }
}
