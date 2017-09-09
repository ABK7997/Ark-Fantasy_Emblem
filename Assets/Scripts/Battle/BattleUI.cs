using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

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

    /***BATTLE PROJECTION***/
    
    /// <summary>A box which is used to display the battle projection stats</summary>
    public Image battleProjection;
    
    /// <summary>Text within the Battle Projection canvas regarding the initiator</summary>
    public Text projectionInfo;

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
                SetProjection(false);
                cancelButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(true);
                break;

            case "ANIMATING":
                break;

            case "COMMANDING":
                SetTargetting(false);
                commandsList.SetActive(true);
                specialSelection.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(false);

                int timer = bm.pParty.GetActiveMember().TechTimer;

                if (timer == 0)
                {
                    techButtonText.text = "TECH";
                    techButtonText.color = Color.black;
                }
                else if (timer == 1)
                {
                    techButtonText.text = "TECH recharging: " + timer + " turn";
                    techButtonText.color = Color.gray;
                }
                else
                {
                    techButtonText.text = "TECH recharging: " + timer + " turns";
                    techButtonText.color = Color.gray;
                }

                break;

            case "SPECIAL_SELECTION":
                commandsList.SetActive(false);
                specialSelection.gameObject.SetActive(true);
                break;

            case "SELECTION":
                SetTargetting(true);
                commandsList.SetActive(false);
                SetProjection(false);
                specialSelection.gameObject.SetActive(false);
                break;

            case "PLAYER_PROJECTION":
                SetTargetting(true);
                SetProjection(true);
                SetTargetting(false);

                //battleProjection.rectTransform.anchorMin = new Vector2(0, 0);
                //battleProjection.rectTransform.anchorMax = new Vector2(0, 0);

                break;

            case "ENEMY_PROJECTION":
                SetProjection(true);
                cancelButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(false);

                //battleProjection.rectTransform.anchorMin = new Vector2(1, 0);
                //battleProjection.rectTransform.anchorMax = new Vector2(1, 0);

                break;

            case "PAUSED":
                pauseScreen.gameObject.SetActive(true);
                break;

            default: break;
        }
    }

    /// <summary>
    /// Physical attack projection. Pauses game and display BP window.
    /// </summary>
    /// <param name="atk">Amount of damage that might be done</param>
    /// <param name="hit">% chance to hit target</param>
    /// <param name="crit">% chance to land a critical on target</param>
    public void SetProjectionInfo(int atk, int hit, int crit)
    {
        SetProjection(true);

        projectionInfo.text =
            "ATK: " + atk + "\n" +
            "HIT: " + hit + "%\n" +
            "CRIT: " + crit + "%\n";
    }

    /// <summary>
    /// Projection method for healing or repair spells/techs
    /// </summary>
    /// <param name="effect">Amount that the target will be healed for</param>
    public void SetProjectionInfo(int effect, int crit)
    {
        SetProjection(true);

        projectionInfo.text =
            "HP Up: " + effect +
            "\nBonus: " + crit + "%";
    }

    //Shorthand to enabling/disabling the Battle Projection game object
    private void SetProjection(bool b)
    {
        battleProjection.gameObject.SetActive(b);
    }

    //Shorthand to enable/disable targetting UI elements
    private void SetTargetting(bool b)
    {
        targetingText.enabled = b;
        targetCancelButton.gameObject.SetActive(b);
    }
}
