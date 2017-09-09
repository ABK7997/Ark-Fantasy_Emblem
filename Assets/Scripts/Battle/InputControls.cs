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
	}

    //Cancellation hotkeys
    private void CancellationKeys()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetMouseButtonDown(1))
        {
            Cancel();
        }
    }

    /// <summary>
    /// Method which can be used by UI buttons and typically sets the battle back one state (i.e. SELECTION to COMMANDs)
    /// </summary>
    public void Cancel()
    {
        switch (bm.GetState())
        {
            case "COMMANDING":
            case "PAUSED":
                bm.SetState("NORMAL");
                break;

            case "SELECTION":
            case "SPECIAL_SELECTION":
                pParty.NullifySpecial();
                bm.SetState("COMMANDING");
                break;

            case "PLAYER_PROJECTION":
                bm.CancelAction();
                bm.SetState("SELECTION");
                pParty.CancelTarget();
                break;
        }
    }

    //OK or affirmative hotkeys
    private void ConfirmationKeys()
    {
        if (Input.GetKeyDown(KeyCode.Return))
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
}
