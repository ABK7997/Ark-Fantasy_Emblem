using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Subclass of Entity. Enemies are always in the opposite party fighting the player
/// </summary>
public class Enemy : Entity {

    //Permanent codes for choosing Spells / Techs
    protected const int ETHEREAL_ARROW = 0, SHOCK = 0;
    protected const int SOUL_SPEAR = 1, FLAMETHROWER = 1;
    protected const int THRASH = 2, PULSE = 2;
    protected const int LIGHTNING = 3, LASER = 3;
    protected const int COMET = 4, REPAIR = 4;
    protected const int METEOR_SHOWER = 5, MASS_REPAIR = 5;
    protected const int HEAL = 6, SHIELD = 6;
    protected const int MEND = 7, BARRIER = 7;
    protected const int SPRINKLE = 8, GROUND = 8;
    protected const int RESUSCITATE = 9, SLOW = 9;
    protected const int POISON = 10, HASTEN = 10;
    protected const int RUST = 11, REBOOT = 11;

    //Keep track of turn number for certain behavior patterns
    protected int turn = 0;

    //Calls base; Starts the enemy's speed bar at a random point between 0% and 50% full
    protected override void Start()
    {
        base.Start();

        moveTimer += Random.Range(0.000f, 50.000f);

        for (int i = 1; i < level; i++)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Calls base; Enemy takes a turn when their speed bar is ready
    /// </summary>
    public override void UpdateTime()
    {
        base.UpdateTime();
    }

    /// <summary>
    /// Method describing how enemies will behave when it's their turn
    /// </summary>
    public virtual void Behavior()
    {
        string command = "ATTACK";
        Entity target = GetRandomPlayer();

        party.ExecuteAction(command, this, target);
        ResetTimer();
    }

    /***BEHAVIORS and TARGETING***/
    //GET PLAYERS
    //Get a random player from the opposite party to target
    protected Entity GetRandomPlayer()
    {
        List<Entity> targets = party.GetLivingEnemies();

        int selection = Random.Range(0, targets.Count);

        //Exposed Target
        foreach (Entity e in targets)
        {
            if (e.ec.CheckEffect("EXPOSED"))
            {
                if (BlowhornEffect())
                {
                    return e;
                }
            }
        }

        //Obscure Target
        if (targets.Count > 1 && targets[selection].ec.CheckEffect("OBSCURE"))
        {
            return ObscureEffect(targets, targets[selection]);
        }

        //Normal Selection
        else return targets[selection]; 
    }

    //EFFECT - OBSCURE
    protected Entity ObscureEffect(List<Entity> targets, Entity previousTarget)
    {
        int chance = 80;

        //Change target
        if (Random.Range(0, 100) <= chance)
        {
            targets.Remove(previousTarget);

            int selection = Random.Range(0, targets.Count);

            targets.Add(previousTarget);

            return targets[selection];
        }

        //Chance failed - same target acquired
        else return previousTarget;
    }

    //EFFECT - BLOWHORN
    protected bool BlowhornEffect()
    {
        int chance = 60;

        if (Random.Range(0, 100) <= chance)
        {
            return true;
        }

        else return false;
    }

    //Get a player by class type
    protected List<Entity> GetTypePlayer(List<Entity> targets, TYPE type)
    {
        List<Entity> retParty = new List<Entity>();

        foreach (Entity e in targets)
        {
            switch (type)
            {
                case TYPE.ORGANIC: if (e.IsOrganic()) retParty.Add(e); break;
                case TYPE.MAGIC: if (e.IsMagic()) retParty.Add(e); break;
                case TYPE.DROID: if (e.IsDroid()) retParty.Add(e); break;
            }
        }

        return retParty;
    }



    //GET ALLIES
    //Get a random ally from this one's enemy party to target
    protected Entity GetRandomAlly()
    {
        int selection = Random.Range(0, party.party.Count);

        return party.party[selection];
    }

    //Get ally with the lowest HP (probably to heal)
    protected Entity GetAllyLowHP()
    {
        Entity ret = null;

        foreach (Entity e in party.GetLivingParty())
        {
            if (e.Hp < ret.Hp
                && e.Index != Index) ret = e;
        }

        return ret;
    }



    //SELECT SPECIALS
    protected void SetSpecial(int index, Special.CLASS classification)
    {
        switch (classification)
        {
            case Special.CLASS.SKILL: bc.activeSpecial = skills[index]; break;
            case Special.CLASS.SPELL: bc.activeSpecial = spells[index]; break;
            case Special.CLASS.TECH: bc.activeSpecial = techs[index]; break;
        }
    }
}
