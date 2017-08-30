using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Contains both parties and manages the flow of battle
/// </summary>
public class BattleManager : MonoBehaviour {

    /***PLAYERS AND ENEMIES***/
    ///<summary>A party containing the player's own characters</summary> 
    public PlayerParty pParty;

    ///<summary>A party containing the enemies the player is fighting</summary>
    public EnemyParty eParty;

    /***FLOW OF BATTLE and UI***/
    ///<summary>The UI which contains elements set apart from party control</summary> 
    public Canvas commands;

    /// <summary>An overlaying, half-transparent image that visually declares to the player when the game is paused or not </summary>
    public Image pauseScreen;

    /// <summary>
    /// The canvas containing all the buttons for issuing player Orders
    /// </summary>
    public GameObject commandsList;

    /// <summary>
    /// Text which appears when the player is choosing a target
    /// </summary>
    public Text targetingText;

    private Queue<Order> actions = new Queue<Order>(); //Stores actions for all entities and performs them in order

    /***BATTLE SATE ***/
    private enum STATE
    {
        NORMAL, ANIMATING, COMMANDING, SELECTION, PLAYER_PROJECTION, ENEMY_PROJECTION, PAUSED, GAME_OVER
    }
    private STATE state = STATE.NORMAL;

    //private bool animating = false; //If true, speed gauges are halted for all entites as an action is performed
    //private bool paused = false; //Similar to animating, but dictated by player manually pausing and unpausing
    
    /***BATTLE PROJECTION***/
    /// <summary>A box which is used to display the battle projection stats</summary>
    public Image battleProjection;
    /// <summary>Text within the Battle Projection canvas regarding the initiator</summary>
    public Text projectionInfoSender;
    /// <summary>Text within the Battle Projection canvas regarding the recipient</summary>
    public Text projectionInfoRecipient;
    /// <summary>
    /// The cancel button in the BP window; unavailable if it is an Enemy Projection
    /// </summary>
    public Button cancelButton;

    /***MAIN METHODS***/

    //Constructs both parties and attaches this battle manager to each of them
    void Start()
    {
        pauseScreen.enabled = false; //Disable pause overlay
        SetProjection(false); //Disable battle projection
        commandsList.SetActive(false);
        targetingText.enabled = false;

        //Organize parties
        pParty.OrganizeParty();
        eParty.OrganizeParty();

        //Assign opposite parties
        pParty.ConstructOppositeParty(eParty.party);
        eParty.ConstructOppositeParty(pParty.party);

        //Assign manager
        pParty.SetBattleManager(this);
        eParty.SetBattleManager(this);
    }
	
    //Controls the flow of battle with Order Queue
	void Update () {

        if (state == STATE.NORMAL) //Check if both not animating and not paused by player7
        {
            StartCoroutine(Animate());

            foreach (Entity e in pParty.party)
            {
                e.UpdateTime();
            }
            foreach (Entity e in eParty.party)
            {
                e.UpdateTime();
            }
        }
    }

    /***ACTIONS***/

    //A struct containing the information necessary for any entity to make a move
    private struct Order
    {
        public string type;
        public Entity user, target;

        public Order(string type, Entity user, Entity target)
        {
            this.type = type;
            this.user = user;
            this.target = target;
        }
    }

    /// <summary>
    /// Creates a new struct, Order, in the Order Queue to line up the next action.
    /// </summary>
    /// <param name="type">Command type: ATTACK, DEFEND, etc. </param>
    /// <param name="user">Entity from which the command was generated</param>
    /// <param name="target">Entity which is being attacked/affected by the user</param>
    public void IssueOrder(string type, Entity user, Entity target)
    {
        Order order = new Order(type, user, target);

        actions.Enqueue(order);
    }
    
    //Start queued actions
    private IEnumerator Animate()
    {
        while (actions.Count != 0)
        {
            state = STATE.ANIMATING;
            EnactOrder();
            yield return new WaitForSeconds(1);
        }

        state = STATE.NORMAL;
    }

    public bool IsAnimating()
    {
        return state == STATE.ANIMATING;
    }

    /// <summary>
    /// Executes the first order on the Order Queue
    /// </summary>
    public void EnactOrder()
    {
        //Get next order
        Order currentOrder = actions.Dequeue();
        string type = currentOrder.type;
        Entity user = currentOrder.user;
        Entity target = currentOrder.target;

        //Perform action
        switch (type)
        {
            case "ATTACK": user.Attack(target); break;
        }
    }

    //Returns true if the game is neither animating an action nor paused by the player
    /*
    private bool Free()
    {
        return !animating && !paused;
    }
    */

    /***BATTLE PROJECTION METHODS***/
    
    /// <summary>
    /// Recieve info from active entity to update the Battle Projection texts; also pauses game and displays BP window
    /// </summary>
    public void SetProjectionInfo(int atk, int hit, int crit)
    {
        SetProjection(true);

        projectionInfoSender.text =
            "ATK: " + atk + "\n" +
            "HIT: " + hit + "%\n" +
            "CRIT: " + crit + "%\n";
    }

    //Shorthand to enabling/disabling the Battle Projection game object
    private void SetProjection(bool b)
    {
        battleProjection.gameObject.SetActive(b);
    }

    /***BUTTON METHODS***/

    /// <summary>
    /// Cancels the player's queued action
    /// </summary>
    public void CancelAction()
    {
        actions.Dequeue();
    }

    /// <summary>
    /// Alter the game's state, such as to pause game or show BP window
    /// </summary>
    /// <param name="state"></param>
    public void SetState(string newState)
    {
        switch (newState)
        {
            case "NORMAL":
                state = STATE.NORMAL;
                pauseScreen.enabled = false;
                commandsList.SetActive(false);
                SetProjection(false);
                eParty.ResetState();
                pParty.ResetState();
                cancelButton.gameObject.SetActive(true);
                break;

            case "ANIMATING": state = STATE.ANIMATING;
                break;

            case "COMMANDING": state = STATE.COMMANDING;
                targetingText.enabled = false;
                commandsList.SetActive(true);
                break;

            case "SELECTION": state = STATE.SELECTION;
                targetingText.enabled = true;
                commandsList.SetActive(false);
                SetProjection(false);
                break;

            case "PAUSED": state = STATE.PAUSED;
                pauseScreen.enabled = true;
                break;

            case "PLAYER_PROJECTION": state = STATE.PLAYER_PROJECTION;
                targetingText.enabled = false;
                SetProjection(true);
                break;

            case "ENEMY_PROJECTION": state = STATE.ENEMY_PROJECTION;
                SetProjection(true);
                cancelButton.gameObject.SetActive(false);
                break;

            default: Debug.Log("Not an existing state: " + newState); break;
        }
    }

    public string GetState()
    {
        return state + "";
    }
}
