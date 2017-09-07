using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    /// <summary>The Battle Manager, mostly just for its GetState() method</summary>
    public BattleManager bm;

    /// <summary>The canvas containing all the buttons for issuing player Orders</summary>
    public GameObject commandsList;

    /// <summary>An overlaying, half-transparent image that visually declares to the player when the game is paused or not </summary>
    public Canvas pauseScreen;

    /// <summary>Text which appears when the player is choosing a target</summary>
    public Text targetingText;

    /// <summary>On-screen button to cancel targetting if the player does not prefer using hotkeys</summary>
    public Button targetCancelButton;

    /***BATTLE PROJECTION***/
    
    /// <summary>A box which is used to display the battle projection stats</summary>
    public Image battleProjection;
    
    /// <summary>Text within the Battle Projection canvas regarding the initiator</summary>
    public Text projectionInfo;

    /// <summary> The cancel button in the BP window; unavailable if it is an Enemy Projection</summary>
    public Button cancelButton;

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
                break;

            case "ANIMATING":
                break;

            case "COMMANDING":
                SetTargetting(false);
                commandsList.SetActive(true);
                break;

            case "SELECTION":
                SetTargetting(true);
                commandsList.SetActive(false);
                SetProjection(false);
                break;

            case "PAUSED":
                pauseScreen.gameObject.SetActive(true);
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

                //battleProjection.rectTransform.anchorMin = new Vector2(1, 0);
                //battleProjection.rectTransform.anchorMax = new Vector2(1, 0);

                break;

            default: break;
        }
    }

    /// <summary>
    /// Recieve info from active entity to update the Battle Projection texts; also pauses game and displays BP window
    /// </summary>
    public void SetProjectionInfo(int atk, int hit, int crit)
    {
        SetProjection(true);

        projectionInfo.text =
            "ATK: " + atk + "\n" +
            "HIT: " + hit + "%\n" +
            "CRIT: " + crit + "%\n";
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
