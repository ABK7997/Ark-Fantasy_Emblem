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
    private int level = 1;

    /// <summary> UI showing stats which appears and disappears as the mouse hovers over entities </summary>
    public Canvas statView;

    /// <summary> Inside of the statView, contains all momentary stat information for a character - constantly updated </summary>
    public Text statText;

    /// <summary>
    /// Typical side-facing sprite
    /// </summary>
    public Sprite normalSprite;

    /// <summary>
    /// Sprite used when PC is using the DEFEND command. Most enemies do not have it
    /// </summary>
    public Sprite defendingSprite;

    /// <summary>
    /// The sprite used for when an entity is KO
    /// </summary>
    public Sprite deathSprite;

    /// <summary> Organic, Magic, or Droid - can be multityped </summary>
    public string type;

    /// <summary> Hit Points </summary>
    public int maxHP; protected int _hp; //Health

    //OFFENSIVE STATS
    /// <summary> Physical strength </summary>
    public int baseAtk; protected int _atk;
    /// <summary> Magical strength </summary>
    public int baseMag; protected int _mag;
    /// <summary> Electrical strength </summary>
    public int baseVlt; protected int _vlt;

    //DEFENSIVE STATS
    /// <summary> Physical resistance </summary>
    public int baseDef; protected int _def;
    /// <summary> Magical resistance </summary>
    public int baseRes; protected int _res;
    /// <summary> Electrical resistance </summary>
    public int baseStb; protected int _stb;

    //PERFORMANCE STATS
    /// <summary> Determines critical hit rate </summary>
    public int baseSkl; protected int _skl;
    /// <summary> Lowers the chance of being hit by a critical </summary>
    public int baseLck; protected int _lck;
    /// <summary> Determines how fast the speed gauge increases </summary>
    public int baseSpd; protected int _spd;

    //Temporary stats
    private int hitChance, critChance, physicalDmg, magicDmg, techDmg;
    private Entity target;

    /// <summary> Enum which keeps track of player statuses such as death or negative status effects</summary>
    [HideInInspector]
    //Conditions and status effects
    public enum STATUS
    {
        NORMAL, ILL, DEFENDING, DEAD
    }

    /// <summary> Status variable </summary>
    [HideInInspector]
    public STATUS status = STATUS.NORMAL;

    //Target Color
    protected Color targeted = Color.red;
    protected Color active = Color.green;
    protected Color normal;
    protected Color hover = Color.gray;

    //Speed
    protected float moveTimer = 0f; //Counts up to 100 over time, and then the entity can act
    protected bool ready = false; //True if moveTimer = 100, false otherwise

    //Components
    protected Animator anim; //The animator used for battle animations
    protected SpriteRenderer render; //Renders sprites
    protected AnimationClip[] clips; //Animator clips

    //STAT DISPLAY
    /// <summary>The canvas containg relevant states, namely HP and speed progress bar</summary>
    public Canvas overhead;
    /// <summary>HP display</summary>
    public Image hpBar;
    /// <summary>Length of speed and health bars</summary>
    public float barsLength;
    /// <summary>Height of speed and health bars</summary>
    public float barsHeight;

    //Modifers
    private float speedMultiplier = 2f; //Basic multiplier to speed up or slow down all combat

    //Sets base stats, components, and initial display
    protected virtual void Start()
    {
        statView.enabled = false;

        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        clips = anim.runtimeAnimatorController.animationClips;

        ResetStats();
        UpdateDisplay();

        normal = render.color; //Set the normal color to the sprite's starting color

        anim.enabled = false;
    }

    void Update()
    {
        switch (status)
        {
            case STATUS.NORMAL: render.sprite = normalSprite; break;
            case STATUS.DEFENDING: render.sprite = defendingSprite; break;
            case STATUS.DEAD: render.sprite = deathSprite; break;

            default: break;
        }
    }

    /// <summary>
    /// Manages speed bars
    /// </summary>
    public virtual void UpdateTime() {
        if (status == STATUS.DEAD) return; //Do nothing if dead

        if (moveTimer < 100) moveTimer += Time.deltaTime + (Spd / (25f) * speedMultiplier);
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
        statText.text = GetAllStats();
    }

    protected void OnMouseExit()
    {
        hovering = false;
        ChangeColor("normal");

        statView.enabled = false;
    }

    /***BATTLE METHODS***/
    /// <summary>
    /// Primary Command; uses physical attack based on ATK stat to harm one other entity
    /// </summary>
    /// <param name="e">Entity to attack - can be friendly</param>
    public void Attack(string type)
    {
        bool landedHit = false;
        bool landedCrit = false;
        int totalDamage = 0;

        //Calculate hit or miss
        if (Random.Range(0, 100) <= hitChance) landedHit = true;
        if (Random.Range(0, 100) <= critChance) landedCrit = true;

        switch (type)
        {
            case "ATTACK": totalDamage = physicalDmg; break;
            case "MAGIC": totalDamage = magicDmg; break;
            case "TECH": totalDamage = techDmg; break;

            default: Debug.Log("Invalid attack type: " + type); break;
        }

        if (landedCrit) totalDamage = (int)(totalDamage * 2.25); //Crit damage
        if (landedHit) target.Hp -= totalDamage; //Hit

        //Animate
        anim.enabled = true;
        anim.SetTrigger("ATTACK");
        ResetTimer();
    }

    /// <summary>
    /// Doubles the user's defensive stats or returns them to normal
    /// </summary>
    /// <param name="b"></param>
    public void SetDefending(bool b)
    {
        if (b)
        {
            Def *= 2;
            Res *= 2;
            Stb *= 2;

            status = STATUS.DEFENDING;
            ResetTimer();
        }
        else
        {
            Def /= 2;
            if (Def < baseDef) Def = baseDef;

            Res /= 2;
            if (Res < baseRes) Res = baseRes;

            Stb /= 2;
            if (Stb < baseStb) Stb = baseStb;

            status = STATUS.NORMAL;
        }
    }

    /***GETTER and SETTER METHODS***/

    /// <summary>
    /// Set and get for HP value
    /// </summary>
    public int Hp
    {
        get { return _hp; }
        set {
            if (status != STATUS.DEFENDING) anim.enabled = true;

            _hp = value; //change

            if (Hp > maxHP) _hp = maxHP;

            //Death
            else if (Hp <= 0)
            {
                _hp = 0;
                status = STATUS.DEAD;
                moveTimer = 0f;
                ready = false;

                anim.enabled = false;
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
    public string GetStatus()
    {
        return status + "";
    }

    /// <summary>
    /// A method which lists all the entity's current stats
    /// </summary>
    /// <returns>A long string with many line breaks to display current statistics</returns>
    public string GetAllStats()
    {
        return Name + ", Lv. " + level + "\n" +
            "HP: " + Hp + "\n" +
            type + "\n\n" +
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

    /***BATTLE PROJECTION INFO***/

    /// <summary>
    /// Change the entity's color overlay
    /// </summary>
    public void ChangeColor(string code)
    {
        Color colorChange;

        switch (code)
        {
            case "target": colorChange = targeted; break;
            case "active": colorChange = active; break;
            case "normal": colorChange = normal; break;
            case "hover": colorChange = hover; break;
            default: Debug.Log("Not a valid color code: " + code); return;
        }

        render.color = colorChange;

    }

    /// <summary>
    /// Method to find the length of the entity's current animation
    /// 0 - IDLE
    /// 1 - ATTACK
    /// 2 - SKILL
    /// 3 - MAGIC
    /// 4 - TECH
    /// 5 - ITEM
    /// </summary>
    /// <returns>length - length (in seconds) of the current animation clip</returns>
    public float GetAnimationTime(string command)
    {
        foreach (AnimationClip c in clips)
        {
            //Debug.Log(c.name + ": " + c.length);
        }

        /*
        switch (command)
        { 
            case "ATTACK": return clips[1].length * 4;
            default: return 1f;
        }
        */

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="crit"></param>
    public void SetTemporaryStats(Entity t)
    {
        target = t;
        hitChance = HitChance();
        critChance = CritChance();

        //Physical Attack Calculation
        physicalDmg = Atk - t.Def;
        if (physicalDmg < 0) physicalDmg = 0;

        //Magic Attack Calculation
        magicDmg = Mag - t.Res;
        if (magicDmg < 0) magicDmg = 0;

        //Tech Attack Calculation
        techDmg = Vlt - t.Stb;
        if (techDmg < 0) techDmg = 0;
    }

    //Accuracy
    protected int AccuracyCalculation()
    {
        int accuracy = 70; //base acc = 70
        accuracy += Skl * 2; // + SKL * 2
        accuracy += Lck; // + LCK

        return accuracy;
    }

    //Evasion
    protected int EvasionCalculation()
    {
        int evade = 0; //base evd = 0
        evade += target.Spd; // + SPD
        evade += target.Lck / 2; // + LCK/2

        return evade;
    }

    //Hit Chance
    protected int HitChance()
    {
        int hit = AccuracyCalculation() - EvasionCalculation();

        if (hit < 0) hit = 0;
        if (hit > 99) hit = 99;

        return hit;
    }

    //Crit Accuracy
    protected int CritAccuracyCalculation()
    {
        int crit = 1;
        crit += Skl / 2;

        return crit;
    }

    //Crit Evasion
    protected int CritEvasionCalcuation()
    {
        int evd = 0;
        evd += target.Lck;

        return evd;
    }

    //Crit Hit Chance
    protected int CritChance()
    {
        int crit = CritAccuracyCalculation() - CritEvasionCalcuation();
        if (crit > 99) crit = 99;
        if (crit < 0) crit = 0;

        return crit;
    }

    //Temporary Stats Getter/Setter

    /// <summary>
    /// Physical damage possible; based on ATK
    /// </summary>
    public int PhysicalDmg {
         get { return physicalDmg; }
         set { physicalDmg = value; }  
    }

    /// <summary>
    /// Magical effectiveness possible; based on MAG
    /// </summary>
    public int MagicDmg
    {
        get { return magicDmg; }
        set { magicDmg = value; }
    }

    /// <summary>
    /// Technical effectivenes possibly; based on VLT
    /// </summary>
    public int TechDmg
    {
        get { return techDmg; }
        set { techDmg = value; }
    }

    /// <summary>
    /// Chance of hitting the intended target
    /// </summary>
    public int Hit
    {
        get { return hitChance; }
        set { hitChance = value; }
    }

    /// <summary>
    /// Chancing of landing a critical multiplier during an attack
    /// </summary>
    public int Crit
    {
        get { return critChance; }
        set { critChance = value; }
    }
}
