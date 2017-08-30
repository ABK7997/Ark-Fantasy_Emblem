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

    //Target Color
    protected Color target = Color.red;
    protected Color active = Color.green;
    protected Color normal = Color.white;
    protected Color hover = Color.gray;

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

        ResetStats();
        UpdateDisplay();
    }

    /// <summary>
    /// Manages speed bars
    /// </summary>
    public virtual void UpdateTime() {
        if (status == STATUS.DEAD) return; //Do nothing if dead

        if (moveTimer < 100) moveTimer += Time.deltaTime * speedMultiplier * Spd;
        else ready = true;
    }

    /***STAT METHODS***/

    /// <summary>
    /// Set all active stats back to their base stats; refill hp
    /// </summary>
    public void ResetStats()
    {
        Hp = maxHP;

        Atk = baseAtk;
        Mag = baseMag;
        Vlt = baseVlt;

        Def = baseDef;
        Res = baseRes;
        Stb = baseStb;

        Skl = baseSkl;
        Lck = baseLck;
        Spd = baseSpd;

        ready = false;
    }

    /// <summary>
    /// Primary Command; uses physical attack based on ATK stat to harm one other entity
    /// </summary>
    /// <param name="e">Entity to attack - can be friendly</param>
    public void Attack(Entity e)
    {
        e.Hp += -Atk;
        anim.SetTrigger("ATTACK");
        ResetTimer();
    }

    /// <summary>
    /// Updates the stats canvas on display in battle
    /// </summary>
    public void UpdateDisplay()
    {
        float hpPercentage = Hp / (float)maxHP;

        hpBar.rectTransform.sizeDelta = new Vector2(barsLength * hpPercentage, barsHeight);
    }

    /// <summary>
    /// Used after Order is carried out to reset the speed bar
    /// </summary>
    public void ResetTimer()
    {
        moveTimer = 0;
        ready = false;

        UpdateDisplay();
    }

    /// <summary>
    /// Used by Party classes to indicate which party the Entity belongs to
    /// </summary>
    /// <param name="belongsTo">The party this entity is a part of</param>
    public void SetParty(Party belongsTo)
    {
        party = belongsTo;
    }

    //<---STATE METHODS**//

    //**AUTOMATIC--->//
    protected void OnMouseOver()
    {
        hovering = true;
        ChangeColor("hover");

        //Show statscreen
        statView.enabled = true;

        //Update stats
        statText.text =
            Name + "\n" +
            "HP: " + Hp + "\n" +
            "Type: " + type + "\n\n" +
            "ATK: " + Atk + "\n" +
            "MAG: " + Mag + "\n" +
            "VLT: " + Vlt + "\n\n" +
            "DEF: " + Def + "\n" +
            "RES: " + Res + "\n" +
            "STB: " + Stb + "\n\n" +
            "SKL: " + Skl + "\n" +
            "LCK: " + Lck + "\n" +
            "SPD: " + Spd + "\n";
    }

    protected void OnMouseExit()
    {
        hovering = false;
        ChangeColor("normal");

        statView.enabled = false;
    }

    /***GETTER and SETTER METHODS***/

    /// <summary>
    /// Set and get for HP value
    /// </summary>
    public int Hp
    {
        get { return _hp; }
        set {
            _hp = value; //change

            if (Hp > maxHP) _hp = maxHP;

            //Death
            else if (Hp < 0)
            {
                _hp = 0;
                status = STATUS.DEAD;
                moveTimer = 0f;
                ready = false;
            }

            UpdateDisplay();
        }
    }

    /// <summary>
    /// Set and get for ATK value
    /// </summary>
    public int Atk
    {
        get { return _atk; }
        set { _atk = value; }
    }

    /// <summary>
    /// Set and get for MAG value
    /// </summary>
    public int Mag
    {
        get { return _mag; }
        set { _mag = value; }
    }

    /// <summary>
    /// Set and get for VLT value
    /// </summary>
    public int Vlt
    {
        get { return _vlt; }
        set { _vlt = value; }
    }

    /// <summary>
    /// Set and get for DEF value
    /// </summary>
    public int Def
    {
        get { return _def; }
        set { _def = value; }
    }

    /// <summary>
    /// Set and get for RES value
    /// </summary>
    public int Res
    {
        get { return _res; }
        set { _res = value; }
    }

    /// <summary>
    /// Set and get for STB value
    /// </summary>
    public int Stb
    {
        get { return _stb; }
        set { _stb = value; }
    }

    /// <summary>
    /// Set and get for SKL value
    /// </summary>
    public int Skl
    {
        get { return _skl; }
        set { _skl = value; }
    }

    /// <summary>
    /// Set and get for LCK value
    /// </summary>
    public int Lck
    {
        get { return _lck; }
        set { _lck = value; }
    }

    /// <summary>
    /// Set and get for SPD value
    /// </summary>
    public int Spd
    {
        get { return _spd; }
        set { _spd = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns> hovering - if the mouse is over the entity or not </returns>
    public bool IsHovering() { return hovering; }

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
    public void ChangeColor(string code)
    {
        Color colorChange;

        switch (code)
        {
            case "target": colorChange = target; break;
            case "active": colorChange = active; break;
            case "normal": colorChange = normal; break;
            case "hover": colorChange = hover; break;
            default: Debug.Log("Not a valid color: " + code); return;
        }

        render.color = colorChange;

    }

}
