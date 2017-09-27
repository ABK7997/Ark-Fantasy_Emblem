using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCalculator {

    Entity user;
    BattleCalculator bc;

    //Effects
    private List<Effect> effects;
    public List<string> immunities;

    public EffectCalculator(Entity u, BattleCalculator calc)
    {
        user = u;
        bc = calc;

        effects = new List<Effect>();
        immunities = new List<string>();
    }

    /// <summary>
    /// Check tile effects at the start of an entity's turn (when the speed gauge is full)
    /// </summary>
    /// <param name="e">The effect to be checked</param>
    public void TileEffectsTurn(Tile.EFFECT e)
    {
        user.Spd = user.baseSpd;

        switch (e)
        {
            //Restore HP
            case Tile.EFFECT.RECOVERY:
                user.Hp += 3;
                break;

            //Speed
            case Tile.EFFECT.STUCK:
                user.Spd /= 2;

                break;
        }
    }

    /// <summary>
    /// Enact the effects of any status effects
    /// </summary>
    public void StatusEffects()
    {
        //TODO
    }

    /// <summary>
    /// Enable the effects of any status effects which affect the entity in a turn-based fashion
    /// </summary>
    public void StatusEffectsTurn()
    {
        //TODO
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
        foreach (Effect e in effects)
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
                case "ARMOR": user.Def = user.baseDef; break;

                default: break;
            }
        }

        if (removal) effects.Remove(toRemove);
    }

    //Cycle through effects and remove if expired
    public void CycleEffects()
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
    public string GetAllEffects()
    {
        string ret = "";

        foreach (Effect e in effects)
        {
            ret += e.EffectName + ": " + e.TurnTimer + "\n";
        }

        return ret;
    }

    //Immunity
    public bool IsImmune(string status)
    {
        return immunities.Contains(status);
    }

}
