using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCalculator {

    Entity user;
    EffectCalculator ec;

    public BattleCalculator(Entity u, EffectCalculator calc)
    {
        user = u;
        ec = calc;
    }

    //Temporary stats
    public int hitChance, critChance, physicalDmg, magicDmg, techDmg, specialCost;
    public bool landedHit, landedCrit;
    public Entity target;
    public Special activeSpecial;
    public int techTimer = 0; //Turns which remain until a tech can be used again

    /***STATS***/
    /// <summary>
    /// Set all active stats back to their base stats; refill hp
    /// </summary>
    public void ResetStats()
    {
        user.Hp = user.maxHP;

        user.Atk = user.baseAtk;
        user.Mag = user.baseMag;
        user.Vlt = user.baseVlt;

        user.Def = user.baseDef;
        user.Res = user.baseRes;
        user.Stb = user.baseStb;

        user.Skl = user.baseSkl;
        user.Lck = user.baseLck;
        user.Spd = user.baseSpd;

        user.Ready = false;

        ec.NullifyAllEffects();
    }

    /***TEMPORARY CALCULATIONS***/

    /// <summary>
    /// Physical effect calculation - damage done by physical ATK 
    /// </summary>
    public void PhysicalEffectCalculation()
    {
        int damage = user.Atk;

        //EFFECT - SWAPPED
        if (ec.CheckEffect("SWAPPED")) damage = user.Mag + user.Vlt;

        int defense = target.Def;

        //Defense Modifiers
        if (target.ec.CheckEffect("ARMOR")) defense = 999; //EFFECT - ARMOR

        physicalDmg = damage - defense;

        //Attack Modifiers
        if (ec.CheckEffect("INTENSE")) physicalDmg *= 3; //EFFECT - INTENSE

        if (physicalDmg < 0) physicalDmg = 0;
    }

    /// <summary>
    /// Magical effect calculation - damage, heal, or none for status ailment
    /// </summary>
    /// <param name="spell">The spell about to be used</param>
    public void MagicEffectCalculation()
    {
        float baseDamage = user.Mag;

        //EFFECT - SWAPPED
        if (ec.CheckEffect("SWAPPED")) baseDamage = user.Atk;

        baseDamage *= activeSpecial.basePwr;

        switch (activeSpecial.type)
        {
            case Special.TYPE.ATTACK:
                baseDamage -= target.Res;
                if (baseDamage < 0) baseDamage = 0;
                break;

            case Special.TYPE.HEAL:
                baseDamage *= -1; //Number becomes negative so the opposite of damage will be given

                //Heal spells CANNOT heal Droids that are not also Organic
                if (!target.IsOrganic() && target.type != Entity.TYPE.MAGIC) baseDamage = 0;

                break;

            case Special.TYPE.EFFECT:
                baseDamage = 0;
                break; //Status ailments do not immediately do damage
        }

        magicDmg = (int)baseDamage;
    }

    /// <summary>
    /// Technical effect calculation. Determines what a user's Tech ability will do
    /// </summary>
    public void TechnicalEffectCalculation()
    {
        float baseDamage = user.Vlt;

        //EFFECT - SWAPPED
        if (ec.CheckEffect("SWAPPED")) baseDamage = user.Atk;

        baseDamage *= activeSpecial.basePwr;

        switch (activeSpecial.type)
        {
            case Special.TYPE.ATTACK:
                baseDamage -= target.Stb;
                if (baseDamage < 0) baseDamage = 0;
                break;

            case Special.TYPE.REPAIR:
                baseDamage *= -1; //Number becomes negative so the opposite of damage will be given

                //Repair spells CANNOT heal non-Droids
                if (!target.IsDroid()) baseDamage = 0;

                break;

            case Special.TYPE.EFFECT:
                baseDamage = 0;
                break; //Status ailments do not immediately do damage
        }

        techDmg = (int)baseDamage;
    }

    /// <summary>
    /// Do battle calculations for all possible moves the entity can make
    /// </summary>
    /// <param name="t">The target entity for an action</param>
    public void SetTemporaryStats(Entity t)
    {
        if (t == null) return;

        target = t;

        //Calculate hit and crit chance
        hitChance = HitChance();
        critChance = CritChance();

        //Physical Attack Calculation
        PhysicalEffectCalculation();

        //Magic Attack Calculation
        if (activeSpecial != null) MagicEffectCalculation();

        //Tech Attack Calculation
        if (activeSpecial != null) TechnicalEffectCalculation();

        if (activeSpecial != null) specialCost = activeSpecial.cost;

        //Tile modifiers
        TileEffects(target.pc.GetTileEffect1());
        TileEffects(target.pc.GetTileEffect2());

        //Calculate hit or miss
        if (Random.Range(0, 100) <= hitChance) landedHit = true;
        if (Random.Range(0, 100) <= critChance) landedCrit = true;
    }

    //Accuracy
    protected int AccuracyCalculation()
    {
        int accuracy;

        if (activeSpecial == null) accuracy = 70; //base acc = 70
        else accuracy = activeSpecial.baseAccuracy;

        accuracy += user.Skl * 2; // + SKL * 2
        accuracy += user.Lck; // + LCK

        //EFFECT - ANGER
        if (ec.CheckEffect("ANGER")) accuracy -= 35;

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

        if (activeSpecial != null)
        {
            if (activeSpecial.type == Special.TYPE.EFFECT) return activeSpecial.baseAccuracy;
        }

        return hit;
    }

    //Crit Accuracy
    protected int CritAccuracyCalculation()
    {
        int crit;

        if (activeSpecial == null) crit = 1;
        else crit = activeSpecial.baseCrit;

        crit += user.Skl / 2;

        //EFFECT - ANGER
        if (ec.CheckEffect("ANGER")) crit += 35;

        return crit;
    }

    //Crit Evasion
    protected int CritEvasionCalcuation()
    {
        if (activeSpecial != null) return 0;

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

    /***SPECIAL TYPES EFFECT***/
    public void OffensiveSpecial()
    {
        if (!landedHit) return;

        float multiplier = 1f;
        if (landedCrit) multiplier = 2.25f;

        switch (activeSpecial.classification)
        {
            case Special.CLASS.SKILL: break; // ???
            case Special.CLASS.SPELL: target.Hp -= (int)(magicDmg * multiplier); break;
            case Special.CLASS.TECH: target.Hp -= (int)(techDmg * multiplier); break;
        }

        if (target.Hp == 0)
        {
            user.Exp += target.expGain; //Gain EXP
        }
    }

    public void HealingSpecial()
    {
        landedHit = true; //Healing can't miss

        float multiplier = 1f;
        if (landedCrit) multiplier = 1.5f;

        target.Hp -= (int)(magicDmg * multiplier);
    }

    public void RepairSpecial()
    {
        landedHit = true; //Repairing can't miss

        float multiplier = 1f;
        if (landedCrit) multiplier = 1.5f;

        target.Hp -= (int)(techDmg * multiplier);
    }

    public void EffectSpecial()
    {
        string eff = activeSpecial.effect + "";

        target.ec.SetEffect(eff, activeSpecial.turnTimer);
    }

    /***EFFECTS***/
    //Check the effects of the tile a target is standing on and apply them
    private void TileEffects(Tile.EFFECT e)
    {
        switch (e)
        {
            //Lose accuracy
            case Tile.EFFECT.HIDDEN:
                hitChance -= 35;
                if (hitChance < 0) hitChance = 0;
                break;

            case Tile.EFFECT.OBSCURED:
                hitChance -= 15;
                if (hitChance < 0) hitChance = 0;
                break;

            //Defense
            case Tile.EFFECT.COVER:
                physicalDmg -= 3;
                if (physicalDmg < 0) physicalDmg = 0;
                break;

            case Tile.EFFECT.FORTIFIED:
                physicalDmg -= 5;
                if (physicalDmg < 0) physicalDmg = 0;
                break;

            //Stability
            case Tile.EFFECT.GROUNDED:
                techDmg = (int)(techDmg * 0.65f);
                if (techDmg < 0) techDmg = 0;
                break;

            case Tile.EFFECT.SOGGY:
                techDmg = (int)(techDmg * 2f);
                break;

            default: break;
        }
    }

    /***GETTERS and SETTERS***/
    /// <summary>
    /// Physical damage possible; based on ATK
    /// </summary>
    public int PhysicalDmg
    {
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
