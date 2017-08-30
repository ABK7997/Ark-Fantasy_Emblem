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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    //Cancellation actions of various kinds
    private void cancellation()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetMouseButtonDown(1))
        {

            //CANCEL
            switch (bm.GetState())
            {
                case "COMMANDING":

                    break;
            }
        }
    }
}
