using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// An essential class containing all the basic information for player characters and enemies alike
/// </summary>
public class Entity : MonoBehaviour {

    /// <summary>Name that will be displayed in battle and stat screens</summary>
    public string Name;

    protected bool hovering = false; //If the mouse if hovering over the entity or not
    protected Party party; //The party this entity belongs to (player or enemy)

    /***STATS***/

    /// <summary> UI showing stats which appears and disappears as the mouse hovers over entities </summary>
    public Canvas statView;

    /// <summary> Inside of the statView, contains all momentary stat information for a character - constantly updated </summary>
    public Text statText;

    /// <summary> Organic, Magic, or Droid - can be multityped </summary>
    public string type;

    /// <summary> Hit Points </summary>
    public int maxHP; protected int _hp; //Health

    //Offensive Stats
    /// <summary> Physical strength </summary>
    public int baseAtk; protected int _atk;
    /// <summary> Magical strength </summary>
    public int baseMag; protected int _mag;
    /// <summary> Electrical strength </summary>
    public int baseVlt; protected int _vlt;

    //Defensive Stats
    /// <summary> Physical resistance </summary>
    public int baseDef; protected int _def;
    /// <summary> Magical resistance </summary>
    public int baseRes; protected int _res;
    /// <summary> Electrical resistance </summary>
    public int baseStb; protected int _stb;

    //Performance Stats
    /// <summary> Determines critical hit rate </summary>
    public int baseSkl; protected int _skl;
    /// <summary> Lowers the chance of being hit by a critical </summary>
    public int baseLck; protected int _lck;
    /// <summary> Determines how fast the speed gauge increases </summary>
    public int baseSpd; protected int _spd;

    /// <summary> Enum which keeps track of player statuses such as death or negative status effects</summary>
    [HideInInspector]
    //Conditions and status effects
    public enum STATUS
    {
        NORMAL, DEAD
    }

    /// <summary> Status variable </summary>
    [HideInInspector]
    public STATUS status = STATUS.NORMAL;

    //Speed
    protected float moveTimer = 0f; //Counts up to 100 over time, and then the entity can act
    protected bool ready = false; //True if moveTimer = 100, false otherwise

    //Components
    protected Animator anim; //The animator used for battle animations
    protected SpriteRenderer render; //Renders sprites

    //Stat Display

    /// <summary>The canvas containg relevant states, namely HP and speed progress bar</summary>
    public Canvas overhead;
    /// <summary>HP display</summary>
    public Image hpBar;
    /// <summary>Length of speed and health bars</summary>
    public float barsLength;
    /// <summary>Height of speed and health bars</summary>
    public float barsHeight;

    //Modifers
    private float speedMultiplier = 5f; //Basic multiplier to speed up or slow down all combat

    //Sets base stats, components, and initial display
    protected virtual void Start()
    {
        statView.enabled = false;

        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        resetStats();
        updateDisplay();
    }

    /// <summary>
    /// Manages speed bars
    /// </summary>
    public virtual void UpdateTime() {
        if (status == STATUS.DEAD) return; //Do nothing if dead

        if (moveTimer < 100) moveTimer += Time.deltaTime * speedMultiplier * spd;
        else ready = true;
    }

    /***STAT METHODS***/

    /// <summary>
    /// Set all active stats back to their base stats; refill hp
    /// </summary>
    public void resetStats()
    {
        hp = maxHP;

        atk = baseAtk;
        mag = baseMag;
        vlt = baseVlt;

        def = baseDef;
        res = baseRes;
        stb = baseStb;

        skl = baseSkl;
        lck = baseLck;
        spd = baseSpd;

        ready = false;
    }

    /// <summary>
    /// Primary Command; uses physical attack based on ATK stat to harm one other entity
    /// </summary>
    /// <param name="e">Entity to attack - can be friendly</param>
    public void attack(Entity e)
    {
        e.hp += -atk;
        anim.SetTrigger("ATTACK");
        resetTimer();
    }

    /// <summary>
    /// Updates the stats canvas on display in battle
    /// </summary>
    public void updateDisplay()
    {
        float hpPercentage = hp / (float)maxHP;

        hpBar.rectTransform.sizeDelta = new Vector2(barsLength * hpPercentage, barsHeight);
    }

    /// <summary>
    /// Used after Order is carried out to reset the speed bar
    /// </summary>
    public void resetTimer()
    {
        moveTimer = 0;
        ready = false;

        updateDisplay();
    }

    /// <summary>
    /// Used by Party classes to indicate which party the Entity belongs to
    /// </summary>
    /// <param name="belongsTo">The party this entity is a part of</param>
    public void setParty(Party belongsTo)
    {
        party = belongsTo;
    }

    //<---STATE METHODS**//

    //**AUTOMATIC--->//
    protected void OnMouseOver()
    {
        hovering = true;
        render.color = Color.gray;

        //Show statscreen
        statView.enabled = true;

        //Update stats
        statText.text =
            Name + "\n" +
            "HP: " + hp + "\n" +
            "Type: " + type + "\n\n" +
            "ATK: " + atk + "\n" +
            "MAG: " + mag + "\n" +
            "VLT: " + vlt + "\n\n" +
            "DEF: " + def + "\n" +
            "RES: " + res + "\n" +
            "STB: " + stb + "\n\n" +
            "SKL: " + skl + "\n" +
            "LCK: " + lck + "\n" +
            "SPD: " + spd + "\n";
    }

    protected void OnMouseExit()
    {
        hovering = false;
        render.color = Color.white;

        statView.enabled = false;
    }

    /***GETTER and SETTER METHODS***/

    /// <summary>
    /// Set and get for HP value
    /// </summary>
    public int hp
    {
        get { return _hp; }
        set {
            _hp = value; //change

            if (hp > maxHP) _hp = maxHP;

            //Death
            else if (hp < 0)
            {
                _hp = 0;
                status = STATUS.DEAD;
                moveTimer = 0f;
                ready = false;
            }

            updateDisplay();
        }
    }

    /// <summary>
    /// Set and get for ATK value
    /// </summary>
    public int atk
    {
        get { return _atk; }
        set { _atk = value; }
    }

    /// <summary>
    /// Set and get for MAG value
    /// </summary>
    public int mag
    {
        get { return _mag; }
        set { _mag = value; }
    }

    /// <summary>
    /// Set and get for VLT value
    /// </summary>
    public int vlt
    {
        get { return _vlt; }
        set { _vlt = value; }
    }

    /// <summary>
    /// Set and get for DEF value
    /// </summary>
    public int def
    {
        get { return _def; }
        set { _def = value; }
    }

    /// <summary>
    /// Set and get for RES value
    /// </summary>
    public int res
    {
        get { return _res; }
        set { _res = value; }
    }

    /// <summary>
    /// Set and get for STB value
    /// </summary>
    public int stb
    {
        get { return _stb; }
        set { _stb = value; }
    }

    /// <summary>
    /// Set and get for SKL value
    /// </summary>
    public int skl
    {
        get { return _skl; }
        set { _skl = value; }
    }

    /// <summary>
    /// Set and get for LCK value
    /// </summary>
    public int lck
    {
        get { return _lck; }
        set { _lck = value; }
    }

    /// <summary>
    /// Set and get for SPD value
    /// </summary>
    public int spd
    {
        get { return _spd; }
        set { _spd = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns> hovering - if the mouse is over the entity or not </returns>
    public bool isHovering() { return hovering; }

    /// <summary>
    /// Set and get for the Ready value determining if the entity can use an action or not
    /// </summary>
    public bool Ready
    {
        get { return ready; }
        set { ready = value; }
    }

    /// <summary>
    /// Getter and setter for STATUS enum
    /// </summary>
    public STATUS Status
    {
        get { return status; }
        set
        {
            status = value;
        }
    }

    /***BATTLE PROJECTION INFO***/

    /// <summary>
    /// Change the entity's color overlay
    /// </summary>
    public void changeColor(Color color)
    {
        render.color = color;
    }

    /// <summary>
    /// Restores the entity's default color
    /// </summary>
    public void restoreColor()
    {
        render.color = Color.white;
    }


}
