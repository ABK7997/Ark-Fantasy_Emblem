using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains both parties and manages the flow of battle
/// </summary>
public class BattleManager : Manager {

    /***GAME AT LARGE***/
    private Overworld ow;

    private EnemyAvatar ea;
    private int numPartyMembers;

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
        BATTLE_PREP, NORMAL, ANIMATING, SPECIAL_ANIMATING,
        COMMANDING, SELECTION, SPECIAL_SELECTION, PLAYER_PROJECTION,
        ENEMY_PROJECTION, PAUSED, GAME_OVER
    }
    private STATE state = STATE.BATTLE_PREP;

    /// <summary>
    /// UI for in-game battles
    /// </summary>
    public BattleUI ui;

    private int hitChance;
    private int critChance;

    /***MAIN METHODS***/
    protected override void Awake()
    {
        base.Awake();

        ow = FindObjectOfType<Overworld>();
        ow.Activate(false);

        board.BoardInit(ow.encounteredParty.bi);

        //Number of PCs allowed in battle
        numPartyMembers = ow.encounteredParty.bi.numPlayers;
    }

    //Constructs both parties and attaches this battle manager to each of them
    void Start()
    {
        //Assign manager
        pParty.SetBattleManager(this, ui);
        eParty.SetBattleManager(this, ui);

        //Organize EnemyParty
        ea = ow.encounteredParty;
        eParty.OrganizeParty(board.enemyCoordinates, board.scaling, ea.GetParty());
    }
	
    //Controls the flow of battle with Order Queue
	void Update () {

        if (state == STATE.NORMAL) //Check if both not animating and not paused by player7
        {
            StartCoroutine(Animate());

            CheckDeath(); //player party KO
            CheckVictory(); //enemy party KO
        }
    }

    /// <summary>
    /// Place PCs on the field after player has selected them from the Battle Prep screen
    /// </summary>
    public void InstantiatePlayerParty(List<Entity> partyMembers)
    {
        //Configure PlayerParty
        pParty.OrganizeParty(board.playerCoordinates, board.scaling, partyMembers);

        //Assign opposite parties
        pParty.ConstructOppositeParty(eParty.party);
        eParty.ConstructOppositeParty(pParty.party);
    }

    /// <summary>
    /// End battle and go to Game Over screen if all the player's party members are dead
    /// </summary>
    public void CheckDeath()
    {
        List<Entity> partyMembers = pParty.GetLivingParty();

        if (partyMembers.Count == 0) SceneManager.LoadScene("game_over");
    }

    /// <summary>
    /// End battle and return to overworld once all enemies are defeated
    /// </summary>
    public void CheckVictory()
    {
        List<Entity> partyMembers = eParty.GetLivingParty();

        if (partyMembers.Count == 0)
        {
            //Revive overworld
            ow.activeEnemies.Remove(ea);
            Destroy(ea.gameObject);
            ow.Activate(true);
            SceneManager.LoadScene("overworld");
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
            float animationTime;

            switch (actions.Peek().type)
            {
                case "ATTACK":
                    SetState("ANIMATING");
                    animationTime = 1f;
                    break;

                case "MAGIC": case "TECH":
                    SetState("SPECIAL_ANIMATING");
                    animationTime = 0f;
                    break;

                default: animationTime = 1.5f; break;
            }

            EnactOrder(); //Next action is dequeued here

            yield return new WaitForSeconds(animationTime);

            if (animationTime > 0f) state = STATE.NORMAL;
        }
    }

    public bool IsAnimating()
    {
        return (state == STATE.ANIMATING || state == STATE.SPECIAL_ANIMATING);
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

        //Perform action
        switch (type)
        {
            case "ATTACK": user.Attack(); break;

            case "SKILL":
            case "MAGIC":
            case "TECH":
                user.Cast(type);
                break;
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
            case "NORMAL": state = STATE.NORMAL;
                eParty.ResetState();
                pParty.ResetState();
                break;

            case "ANIMATING": state = STATE.ANIMATING;
                break;

            case "SPECIAL_ANIMATING": state = STATE.SPECIAL_ANIMATING;
                break;

            case "COMMANDING": state = STATE.COMMANDING;
                break;

            case "SPECIAL_SELECTION": state = STATE.SPECIAL_SELECTION;
                break;

            case "SELECTION": state = STATE.SELECTION;
                break;

            case "PLAYER_PROJECTION": state = STATE.PLAYER_PROJECTION;
                break;

            case "ENEMY_PROJECTION": state = STATE.ENEMY_PROJECTION;
                break;

            case "PAUSED": state = STATE.PAUSED;
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

    /***MISCELLANEOUS***/

    /// <summary>
    /// Get the number of party members allowed from the player in the battle
    /// </summary>
    /// <returns>numPartyMembers - the party count allowed by the Board</returns>
    public int GetNumMembers()
    {
        return numPartyMembers;
    }
}
