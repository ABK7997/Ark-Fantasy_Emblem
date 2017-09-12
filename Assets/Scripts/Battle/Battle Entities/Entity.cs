using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An essential class containing all the basic information for player characters and enemies alike
/// </summary>
public class Entity : MonoBehaviour {

    /// <summary>Name that will be displayed in battle and stat screens</summary>
    public string Name;

    //The ID number of this entity in its party
    private int index;

    //Position to rest to
    private Vector3 originalPosition;

    //Tiles
    private Tile tile, tileProspect;

    protected bool hovering = false; //If the mouse if hovering over the entity or not
    protected Party party; //The party this entity belongs to (player or enemy)

    /***STATS***/
    private int level = 1;

    /// <summary> UI showing stats which appears and disappears as the mouse hovers over entities </summary>
    public Canvas statView;

    /// <summary> Inside of the statView, contains all momentary stat information for a character - constantly updated </summary>
    public Text statText;

    //Battle Calculation class
    private BattleCalculator bc;

    /// <summary> Organic, Magic, or Droid - can be multityped </summary>
    public enum TYPE
    {
        ORGANIC, MAGIC, DROID, ORGANIC_MAGIC, DROID_ORGANIC, MAGIC_DROID
    }
    public TYPE type;

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

    //SPECIALS
    public List<Special> skills;
    public List<Special> spells;
    public List<Special> techs;

    /*
    //Temporary stats
    private int hitChance, critChance, physicalDmg, magicDmg, techDmg, specialCost;
    private bool landedHit, landedCrit;
    private Entity target;
    private Special activeSpecial;
    private int techTimer = 0; //Turns which remain until a tech can be used again
    */

    //Effects
    private List<Effect> effects;
    public List<string> immunities;

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
    /// <summary>The textbox which displays the index number of this entity in its party</summary>
    public Text indexText;
    /// <summary>Length of speed and health bars</summary>
    public float barsLength;
    /// <summary>Height of speed and health bars</summary>
    public float barsHeight;

    //Modifers
    private float speedMultiplier = 2f; //Basic multiplier to speed up or slow down all combat

    //Sets base stats, components, and initial display
    protected virtual void Start()
    {
        bc = new BattleCalculator(this);

        statView.enabled = false;

        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        bc.ResetStats();
        UpdateDisplay();

        normal = render.color; //Set the normal color to the sprite's starting color
    }

    /// <summary>
    /// Manages speed bars
    /// </summary>
    public virtual void UpdateTime() {
        if (status == STATUS.DEAD) return; //Do nothing if dead

        ResetPosition();

        if (moveTimer < 100) moveTimer += Time.deltaTime + (Spd / (25f) * speedMultiplier);
        else Ready = true;
    }

    /***STAT METHODS***/
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
        TechTimer--;
        CycleEffects();

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
    public void Attack()
    {
        SetDefending(false);
        FlipTowardsTarget(bc.target);

        int totalDamage = 0;

        totalDamage = bc.physicalDmg;
        anim.SetTrigger("ATTACK");

        if (bc.landedCrit) totalDamage = (int)(totalDamage * 2.25); //Crit damage
        if (bc.landedHit) bc.target.Hp -= totalDamage; //Hit

        //Miss Animation
        else Miss();

        ResetTimer();
    }

    /// <summary>
    /// Begin the casting of a spell, tech, or skill
    /// </summary>
    /// <param name="type">The type of special ability being used</param>
    public void Cast(string type)
    {
        SetDefending(false);
        FlipTowardsTarget(bc.target);

        if (type == "MAGIC") Hp -= bc.specialCost;
        else if (type == "TECH") bc.techTimer += bc.specialCost + 1; //Add 1 because 1 turn will immediately be reducted after the turn ends

        anim.SetTrigger("SPECIAL");
        bc.activeSpecial.StartAnimation(this, bc.target, bc.landedHit);
        ResetTimer();
    }

    /// <summary>
    /// A special's effect when it finally hits its target
    /// </summary>
    public void SpecialEffect()
    {
        switch (bc.activeSpecial.type)
        {
            case Special.TYPE.ATTACK: OffensiveSpecial(); break;
            case Special.TYPE.HEAL: HealingSpecial(); break;
            case Special.TYPE.REPAIR: RepairSpecial(); break;
            case Special.TYPE.EFFECT: EffectSpecial(); break;

            default: break;
        }

        party.Normalize();
    }

    //Special Types
    private void OffensiveSpecial()
    {
        if (!bc.landedHit) return;

        float multiplier = 1f;
        if (bc.landedCrit) multiplier = 2.25f;

        switch (bc.activeSpecial.classification)
        {
            case Special.CLASS.SKILL: break;
            case Special.CLASS.SPELL: bc.target.Hp -= (int)(bc.magicDmg * multiplier); break;
            case Special.CLASS.TECH: bc.target.Hp -= (int)(bc.techDmg * multiplier); break;
        }
    }

    private void HealingSpecial()
    {
        bc.landedHit = true; //Healing can't miss

        float multiplier = 1f;
        if (bc.landedCrit) multiplier = 1.5f;

        bc.target.Hp -= (int)(bc.magicDmg * multiplier);
    }

    private void RepairSpecial()
    {
        bc.landedHit = true; //Repairing can't miss

        float multiplier = 1f;
        if (bc.landedCrit) multiplier = 1.5f;

        bc.target.Hp -= (int)(bc.techDmg * multiplier);
    }

    private void EffectSpecial()
    {
        string eff = bc.activeSpecial.effect + "";

        bc.target.SetEffect(eff, bc.activeSpecial.turnTimer);
    }

    //Miss Animation
    private void Miss()
    {
        if (IsRightOf(bc.target)) bc.target.SetPosition(2f, 0f);
        else bc.target.SetPosition(-2f, 0f);
    }

    /// <summary>
    /// Check tile effects at the start of an entity's turn (when the speed gauge is full)
    /// </summary>
    /// <param name="e">The effect to be checked</param>
    public void TileEffectsTurn(Tile.EFFECT e)
    {
        Spd = baseSpd;

        switch (e)
        {
            //Restore HP
            case Tile.EFFECT.RECOVERY:
                Hp += 3;
                break;

            //Speed
            case Tile.EFFECT.STUCK:
                Spd /= 2;

                break;
        }
    }

    private void StatusEffects()
    {
        //TODO
    }

    //Check status effects each turn
    private void StatusEffectsTurn()
    {
        //TODO
    }

    /// <summary>
    /// Set temporary stats in this entity's Battle Calculator class
    /// </summary>
    /// <param name="t">The target entity for an action</param>
    public void SetTemporaryStats(Entity t)
    {
        bc.SetTemporaryStats(t);
    }

    //Defense
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
            anim.SetBool("Defending", true);
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
            anim.SetBool("Defending", false);
        }
    }

    /***POSITION and TILES***/

    /// <summary>
    /// Move entity to temporary position on the battlefield
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(transform.position.x + x, transform.position.y + y, 0);
    }

    /// <summary>
    /// Reset entity to original position
    /// </summary>
    public void ResetPosition()
    {
        transform.position = originalPosition;
    }

    /// <summary>
    /// Change this entity's reset position
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public void SetOriginalPosition(float x, float y)
    {
        originalPosition = new Vector3(x, y, 0);
    }

    /// <summary>
    /// Determines if the target entity is to the right of the user entity
    /// </summary>
    /// <param name="user">User entity</param>
    /// <param name="target">Target entity</param>
    /// <returns>True - target x is greater than user x; False - otherwise</returns>
    public bool IsRightOf(Entity target)
    {
        return transform.position.x < target.transform.position.x;
    }

    private bool IsLeftOf(Entity target)
    {
        return transform.position.x > target.transform.position.x;
    }

    //Flip entity if the target is on their opposite side
    private void FlipTowardsTarget(Entity target)
    {
        if (IsRightOf(target))
        {
            if (render.flipX) render.flipX = false;
            if (!target.GetRender().flipX) target.GetRender().flipX = true;
        }
        else if (IsLeftOf(target))
        {
            if (!render.flipX) render.flipX = true;
            if (target.GetRender().flipX) target.GetRender().flipX = false;
        }
    }

    /// <summary>
    /// Set or change the tile this entity is standing on
    /// </summary>
    /// <param name="t">The new tile to be set</param>
    public void SetTile(Tile t)
    {
        tile = t;
        //transform.position = tile.transform.position;
    }

    /// <summary>
    /// The tile this entity is standing on
    /// </summary>
    /// <returns>A board tile</returns>
    public Tile GetTile()
    {
        return tile;
    }

    /// <summary>
    /// The tile this entity might move to if the player decides to
    /// </summary>
    /// <param name="t">The prospective tile</param>
    public void SetTileProspect(Tile t)
    {
        tileProspect = t;
    }

    /// <summary>
    /// Change this entity's position and tile on the board
    /// </summary>
    public void Move()
    {
        tile = tileProspect;
        tileProspect = null;

        transform.position = new Vector3(tile.transform.localPosition.x, tile.transform.localPosition.y, 0);
        originalPosition = transform.position;

        ResetTimer();
    }

    /// <summary>
    /// Get the first status effect of this entity's occupied tile
    /// </summary>
    /// <returns>The status effect of the tile</returns>
    public Tile.EFFECT GetTileEffect1()
    {
        return tile.effect1;
    }

    /// <summary>
    /// Get the second status effect of this entity's occupied tile. Many tiles do not have a second effect
    /// </summary>
    /// <returns>The status effect of the tile</returns>
    public Tile.EFFECT GetTileEffect2()
    {
        return tile.effect2;
    }

    /***GETTER and SETTER METHODS***/
    /// <summary>
    /// Set and get for the index number of this entity in its party
    /// </summary>
    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

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
            else if (Hp <= 0)
            {
                _hp = 0;
                status = STATUS.DEAD;
                moveTimer = 0f;
                ready = false;

                AnimationOff();
                anim.SetBool("Dead", true);
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
        set {
            StatusEffectsTurn();
            TileEffectsTurn(tile.effect1);
            TileEffectsTurn(tile.effect2);
            ready = value;
        }
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
            "SPD: " + Spd + "\n" +
            "\n" +
            GetAllEffects();
    }

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

    //Temporary Stats Getter/Setter

    /// <summary>
    /// Physical damage possible; based on ATK
    /// </summary>
    public int PhysicalDmg {
        get { return bc.physicalDmg; }
        set { bc.physicalDmg = value; }
    }

    /// <summary>
    /// Magical effectiveness possible; based on MAG
    /// </summary>
    public int MagicDmg
    {
        get { return bc.magicDmg; }
        set { bc.magicDmg = value; }
    }

    /// <summary>
    /// Technical effectivenes possibly; based on VLT
    /// </summary>
    public int TechDmg
    {
        get { return bc.techDmg; }
        set { bc.techDmg = value; }
    }

    /// <summary>
    /// Chance of hitting the intended target
    /// </summary>
    public int Hit
    {
        get { return bc.hitChance; }
        set { bc.hitChance = value; }
    }

    /// <summary>
    /// Chancing of landing a critical multiplier during an attack
    /// </summary>
    public int Crit
    {
        get { return bc.critChance; }
        set { bc.critChance = value; }
    }

    /// <summary>
    /// The number of turns remaining until a Tech ability can be used again
    /// </summary>

    public int TechTimer
    {
        get
        {
            return bc.techTimer;
        }
        set
        {
            bc.techTimer = value;

            if (bc.techTimer < 0) bc.techTimer = 0;
        }
    }

    /// <summary>
    /// Set the spell the user may use next
    /// </summary>
    /// <param name="spell">A spell to be cast</param>
    public void SetSpecial(int index, string type)
    {
        switch (type)
        {
            case "SKILL": bc.activeSpecial = skills[index]; break;
            case "MAGIC": bc.activeSpecial = spells[index]; break;
            case "TECH": bc.activeSpecial = techs[index]; break;

            case "NULL": bc.activeSpecial = null; break;

            default: break;
        }
    }

    /// <summary>
    /// Returns the special ability the entity is currently using
    /// </summary>
    /// <returns>The active special ability</returns>
    public Special GetSpecial()
    {
        return bc.activeSpecial;
    }

    /// <summary>
    /// Return the game to state NORMAL
    /// </summary>
    public void Normalize()
    {
        party.Normalize();
    }

    /***STATUS EFFECTS***/
    /// <summary>
    /// Set all possible status effecst to false
    /// </summary>
    public void NullifyAllEffects()
    {
        effects = new List<Effect>();
    }

    /// <summary>
    /// Check if an entity has a certain status effect active
    /// </summary>
    /// <param name="status">The effect to be checked for</param>
    /// <returns></returns>
    public bool CheckEffect(string status)
    {
        foreach(Effect e in effects)
        {
            if (e.EffectName == status) return true;
        }

        return false;
    }

    /// <summary>
    /// Enable or disable a status effect
    /// </summary>
    /// <param name="status">The status to be altered</param>
    /// <param name="set">Enable or disable</param>
    public void SetEffect(string status, int turns)
    {
        Effect nEff = new Effect(status, turns);

        if (CheckEffect(status)) //Effect is already active; reset effect
        {
            DisableEffect(status);
        }

        if (!IsImmune(status)) effects.Add(nEff);
    }

    /// <summary>
    /// Nullify a status effect, good or bad
    /// </summary>
    /// <param name="status">The effect to be nullified</param>
    public void DisableEffect(string status)
    {
        Effect toRemove = new Effect("", 0);
        bool removal = false;

        foreach (Effect e in effects)
        {
            if (e.EffectName == status)
            {
                toRemove = e;
                removal = true;
            }

            //Certain effects
            switch (status)
            {
                case "ARMOR": Def = baseDef; break;

                default: break;
            }
        }

        if (removal) effects.Remove(toRemove);
    }

    //Cycle through effects and remove if expired
    private void CycleEffects()
    {
        List<string> toRemove = new List<string>();

        foreach (Effect e in effects)
        {
            e.Turn();

            if (e.TurnTimer == 0)
            {
                toRemove.Add(e.EffectName);
            }
        }

        foreach (string s in toRemove)
        {
            DisableEffect(s);
        }
    }

    //List all effects in a string
    private string GetAllEffects()
    {
        string ret = "";

        foreach(Effect e in effects)
        {
            ret += e.EffectName + ": " + e.TurnTimer + "\n";
        }

        return ret;
    }

    //Immunity
    private bool IsImmune(string status)
    {
        return immunities.Contains(status);
    }

    //Type Methods

    /// <summary>
    /// Determines if the entity is any of the Organic types
    /// </summary>
    /// <returns>True - if entity type is Organic, Droid/Organic, or Magic/Organic; False - otherwise</returns>
    public bool IsOrganic()
    {
        return
            type == TYPE.ORGANIC ||
            type == TYPE.DROID_ORGANIC ||
            type == TYPE.ORGANIC_MAGIC;
    }

    /// <summary>
    /// Determines if the entity is of any of the Magic types
    /// </summary>
    /// <returns>True - if entity type is Magic, Magic/Droid, or Magic/Organic; False - otherwise</returns>
    public bool IsMagic()
    {
        return
            type == TYPE.MAGIC ||
            type == TYPE.MAGIC_DROID ||
            type == TYPE.ORGANIC_MAGIC;
    }

    /// <summary>
    /// Determines if the entity is of any of the Droid types
    /// </summary>
    /// <returns>True - if entity type is Droid, Droid/Magic, or Droid/Organicc; False - otherwise</returns>
    public bool IsDroid()
    {
        return
            type == TYPE.DROID ||
            type == TYPE.DROID_ORGANIC ||
            type == TYPE.MAGIC_DROID;
    }

    //Animation State

    public void AnimationOff()
    {
        anim.SetBool("Dead", false);
        anim.SetBool("Defending", false);
    }

    /***MISCELLANEOUS***/

    /// <summary>
    /// Get the party class this entity belongs to
    /// </summary>
    /// <returns>This entity's party</returns>
    public Party GetParty()
    {
        return party;
    }

    public SpriteRenderer GetRender()
    {
        return render;
    }
}
