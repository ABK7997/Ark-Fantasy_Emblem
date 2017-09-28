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

            case "SPECIAL_SELECTION":
                SpecialSelectHotkeys();
                return;

            case "SELECTION":
                TargetHotKeys();
                break;
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

        if (pParty.GetActiveMember() != null)
        {
            if (pParty.GetActiveMember().GetSpecial() != null)
            {
                type = pParty.GetActiveMember().GetSpecial().type + "";
            }
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

            case "TILE_SELECTION":
                bm.SetState("COMMANDING");
                break;

            case "PLAYER_PROJECTION":

                bm.CancelAction();

                //Moving to tile
                if (BattleUI.moving)
                {
                    BattleUI.moving = false;
                    bm.SetState("TILE_SELECTION");
                    break;
                }

                //Hit-All special
                if (pParty.GetActiveMember().GetSpecial() != null)
                {
                    if (pParty.GetActiveMember().GetSpecial().hitAll)
                    {
                        bm.SetState("SPECIAL_SELECTION");
                        break;
                    }
                }
                
                //Other
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
            Input.GetKeyDown(KeyCode.V) ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
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
                StartCoroutine(bm.Animate());
                break;

            case "LEVEL_UP":
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

    //SetSpecial Selection Hotkeys
    private void SpecialSelectHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) pParty.SetSpecial(0);
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) pParty.SetSpecial(1);
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) pParty.SetSpecial(2);
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) pParty.SetSpecial(3);
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) pParty.SetSpecial(4);
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)) pParty.SetSpecial(5);
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) pParty.SetSpecial(6);
        if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) pParty.SetSpecial(7);
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) pParty.SetSpecial(8);
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.KeypadDivide)) pParty.SetSpecial(9);
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMultiply)) pParty.SetSpecial(10);
    }

    //Player Selection Hotkeys
    private void PartySelectHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) pParty.ActivateByIndex(0);
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) pParty.ActivateByIndex(1);
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) pParty.ActivateByIndex(2);
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) pParty.ActivateByIndex(3);
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) pParty.ActivateByIndex(4);
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)) pParty.ActivateByIndex(5);
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

    //Target Phase hotkeys
    private void TargetHotKeys()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1)) pParty.TargetEnemyByIndex(0);
        if (Input.GetKeyDown(KeyCode.Keypad2)) pParty.TargetEnemyByIndex(1);
        if (Input.GetKeyDown(KeyCode.Keypad3)) pParty.TargetEnemyByIndex(2);
        if (Input.GetKeyDown(KeyCode.Keypad4)) pParty.TargetEnemyByIndex(3);
        if (Input.GetKeyDown(KeyCode.Keypad5)) pParty.TargetEnemyByIndex(4);
        if (Input.GetKeyDown(KeyCode.Keypad6)) pParty.TargetEnemyByIndex(5);
        if (Input.GetKeyDown(KeyCode.Keypad7)) pParty.TargetEnemyByIndex(6);
        if (Input.GetKeyDown(KeyCode.Keypad8)) pParty.TargetEnemyByIndex(7);
        if (Input.GetKeyDown(KeyCode.Keypad9)) pParty.TargetEnemyByIndex(8);

        if (Input.GetKeyDown(KeyCode.Alpha1)) pParty.TargetAllyByIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) pParty.TargetAllyByIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) pParty.TargetAllyByIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) pParty.TargetAllyByIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) pParty.TargetAllyByIndex(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) pParty.TargetAllyByIndex(5);
    }
}
