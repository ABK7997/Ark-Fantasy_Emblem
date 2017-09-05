using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains both parties and manages the flow of battle
/// </summary>
public class BattleManager : Manager {

    /***PLAYERS AND ENEMIES***/
    ///<summary>A party containing the player's own characters</summary> 
    public PlayerParty pParty;

    ///<summary>A party containing the enemies the player is fighting</summary>
    public EnemyParty eParty;

    ///<summary> Battle Board </summary>
    public BoardManager board;

    /***FLOW OF BATTLE***/
    private Queue<Order> actions = new Queue<Order>(); //Stores actions for all entities and performs them in order

    /***BATTLE SATE ***/
    private enum STATE
    {
        NORMAL, ANIMATING, COMMANDING, SELECTION, PLAYER_PROJECTION, ENEMY_PROJECTION, PAUSED, GAME_OVER
    }
    private STATE state = STATE.NORMAL;

    /// <summary>
    /// UI for in-game battles
    /// </summary>
    public BattleUI ui;

    //Active entity
    private Entity activeEntity;
    private string activeCommand;

    private int hitChance;
    private int critChance;

    /***MAIN METHODS***/
    protected override void Awake()
    {
        base.Awake();
    }

    //Constructs both parties and attaches this battle manager to each of them
    void Start()
    {
        //Assign manager
        pParty.SetBattleManager(this, ui);
        eParty.SetBattleManager(this, ui);

        //Organize parties
        pParty.OrganizeParty(board.playerCoordinates, board.scaling);
        eParty.OrganizeParty(board.enemyCoordinates, board.scaling);

        //Assign opposite parties
        pParty.ConstructOppositeParty(eParty.party);
        eParty.ConstructOppositeParty(pParty.party);
    }
	
    //Controls the flow of battle with Order Queue
	void Update () {

        if (state == STATE.NORMAL) //Check if both not animating and not paused by player7
        {
            StartCoroutine(Animate());

            CheckDeath(); //player party KO
        }
    }

    /// <summary>
    /// End battle and go to Game Over screen if all the player's party members are dead
    /// </summary>
    public void CheckDeath()
    {
        List<Entity> partyMembers = pParty.GetLivingParty();

        if (partyMembers.Count == 0) SceneManager.LoadScene("game_over");

        return;
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
        activeEntity = user;
        activeCommand = type;

        actions.Enqueue(order);
    }
    
    //Start queued actions
    private IEnumerator Animate()
    {
        while (actions.Count != 0)
        {
            state = STATE.ANIMATING;
            EnactOrder();
            yield return new WaitForSeconds(1f);
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
            case "ATTACK": user.Attack(type); break;
        }
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
                eParty.ResetState();
                pParty.ResetState();
                break;

            case "ANIMATING": state = STATE.ANIMATING;
                break;

            case "COMMANDING": state = STATE.COMMANDING;
                break;

            case "SELECTION": state = STATE.SELECTION;
                break;

            case "PAUSED": state = STATE.PAUSED;
                break;

            case "PLAYER_PROJECTION": state = STATE.PLAYER_PROJECTION;
                break;

            case "ENEMY_PROJECTION": state = STATE.ENEMY_PROJECTION;
                break;

            default: Debug.Log("Not an existing state: " + newState); break;
        }

        ui.ChangingState();
    }

    /// <summary>
    /// Get the current game state; used frequently by other classes
    /// </summary>
    /// <returns>state - the game state of the battle</returns>
    public string GetState()
    {
        return state + "";
    }


}
