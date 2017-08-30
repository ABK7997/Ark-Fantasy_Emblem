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

    /***FLOW OF BATTLE***/
    ///<summary>The UI which contains elements set apart from party control</summary> 
    public Canvas commands;

    /// <summary>An overlaying, half-transparent image that visually declares to the player when the game is paused or not </summary>
    public Image pauseScreen;

    private Queue<Order> actions = new Queue<Order>(); //Stores actions for all entities and performs them in order

    private bool animating = false; //If true, speed gauges are halted for all entites as an action is performed
    private bool paused = false; //Similar to animating, but dictated by player manually pausing and unpausing

    /***BATTLE PROJECTION***/
    /// <summary>A box which is used to display the battle projection stats</summary>
    public Image battleProjection;
    /// <summary>Text within the Battle Projection canvas regarding the initiator</summary>
    public Text projectionInfoSender;
    /// <summary>Text within the Battle Projection canvas regarding the recipient</summary>
    public Text projectionInfoRecipient;

    /***MAIN METHODS***/

    //Constructs both parties and attaches this battle manager to each of them
    void Start()
    {
        pauseScreen.enabled = false; //Disable pause overlay
        setProjection(false); //Disable battle projection

        //Organize parties
        pParty.OrganizeParty();
        eParty.OrganizeParty();

        //Assign opposite parties
        pParty.constructOppositeParty(eParty.party);
        eParty.constructOppositeParty(pParty.party);

        //Assign manager
        pParty.setBattleManager(this);
        eParty.setBattleManager(this);
    }
	
    //Controls the flow of battle with Order Queue
	void Update () {

        if (free()) //Check if both not animating and not paused by player7
        {
            StartCoroutine(animate());

            foreach (Entity e in pParty.party)
            {
                e.UpdateTime();
            }
            foreach (Entity e in eParty.party)
            {
                e.UpdateTime();
            }
        }

        if (!animating && Input.GetKeyDown(KeyCode.Space)) togglePause(); //Use spacebar toggle pause
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
    public void issueOrder(string type, Entity user, Entity target)
    {
        Order order = new Order(type, user, target);

        actions.Enqueue(order);
    }
    
    //Start queued actions
    private IEnumerator animate()
    {
        while (actions.Count != 0)
        {
            animating = true;
            enactOrder();
            yield return new WaitForSeconds(1);
        }

        animating = false;
    }

    /// <summary>
    /// Access the animating variable from other classes
    /// </summary>
    /// <returns>animating - if an animation is occuring or not</returns>
    public bool isAnimating()
    {
        return animating;
    }

    /// <summary>
    /// Executes the first order on the Order Queue
    /// </summary>
    public void enactOrder()
    {
        //Get next order
        Order currentOrder = actions.Dequeue();
        string type = currentOrder.type;
        Entity user = currentOrder.user;
        Entity target = currentOrder.target;

        //Perform action
        switch (type)
        {
            case "ATTACK": user.attack(target); break;
        }
    }

    //Returns true if the game is neither animating an action nor paused by the player
    private bool free()
    {
        return !animating && !paused;
    }

    /***BATTLE PROJECTION METHODS***/
    
    /// <summary>
    /// Recieve info from active entity to update the Battle Projection texts; also pauses game and displays BP window
    /// </summary>
    public void setProjectionInfo(int atk, int hit, int crit)
    {
        setProjection(true);

        projectionInfoSender.text =
            "ATK: " + atk + "\n" +
            "HIT: " + hit + "%\n" +
            "CRIT: " + crit + "%\n";

        setPause(true);
    }

    //Shorthand to enabling/disabling the Battle Projection game object
    private void setProjection(bool b)
    {
        battleProjection.gameObject.SetActive(b);
    }

    /***BUTTON METHODS***/

    /// <summary>
    /// Switch between pause and unpaused; also makes use of the pause screen
    /// </summary>
    public void togglePause()
    {
        if (animating) return; //ignore method if an animation is occurring
        paused = !paused;
        pauseScreen.enabled = !pauseScreen.enabled;
    }

    /// <summary>
    /// Used to pause the game for reasons outside of manual pausing - such as when the player is choosing an entity command
    /// </summary>
    /// <param name="b"></param>
    public void setPause(bool b)
    {
        if (animating) return; //ignore method if an animation is occurring
        paused = b;
    }

    /// <summary>
    /// Continues battle after BP window appears; used manually by player
    /// </summary>
    public void okayButton()
    {
        setProjection(false);
        setPause(false);
        pParty.resumeBattle();
        eParty.resetState();
    }

    /// <summary>
    /// Cancels the player's action or backs out of the BP window
    /// </summary>
    public void cancel()
    {
        if (pParty.getState() == "ENEMY_PROJECTION") return; //cancel button nonfunctional during enemy move

        actions.Dequeue();
        setProjection(false);
        pParty.startSelection();
    }
}
