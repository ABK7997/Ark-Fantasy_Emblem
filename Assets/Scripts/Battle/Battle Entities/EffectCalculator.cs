using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCalculator {

    Entity user;
    //BattleCalculator bc;
    PositionCalculator pc;

    //Effects
    private List<Effect> effects;

    /// <summary>
    /// An assistant class for Entity which calculates all tile and status effects
    /// </summary>
    /// <param name="u">The entity this caluclator belongs to</param>
    /// <param name="calc">The Battle Calculator</param>
    /// <param name="pos">The Position Calculator</param>
    public EffectCalculator(Entity u, BattleCalculator calc, PositionCalculator pos)
    {
        user = u;
        //bc = calc;
        pc = pos;

        effects = new List<Effect>();
    }

    /// <summary>
    /// Check tile effects at the start of an entity's turn (when the speed gauge is full)
    /// </summary>
    /// <param name="e">The effect to be checked</param>
    public void TileEffectsTurn(Tile.EFFECT e)
    {
        switch (e)
        {
            //Restore HP
            case Tile.EFFECT.RECOVERY:
                user.Hp += 3;
                break;

            //Lose HP
            case Tile.EFFECT.HAZARD:
                user.Hp -= 3;
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
        //Lose HP
        if (CheckEffect("POISON")) user.Hp -= 3;
        if (CheckEffect("CORROSION")) user.Hp -= 4;
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
        switch (status)
        {
            //Unique Case - Revive
            case "REVIVE":
                user.SetStatus("NORMAL");
                user.Hp += (int)(user.maxHP * 0.33f);
                user.GetParty().UpdateIndeces();
                return;

            //Unique Case - Reset
            case "RESET":
                NullifyAllEffects();
                user.bc.ResetStats();
                return;

            //Swift absolves Slow
            case "SWIFT": DisableEffect("SLOW"); break;

            //Slow eliminates Swift
            case "SLOW": DisableEffect("SWIFT"); break;
        }

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

    /// <summary>
    /// Cycle through effects and remove if expired
    /// </summary>
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

    /// <summary>
    /// List all status and tile effects on this entity in a string
    /// </summary>
    /// <returns></returns>
    public string GetAllEffects()
    {
        string ret = "";

        foreach (Effect e in effects)
        {
            ret += e.EffectName + ": " + e.TurnTimer + "\n";
        }

        if (pc.tile.effect1 != Tile.EFFECT.NONE)
        {
            ret += pc.tile.effect1;

            if (pc.tile.effect2 != Tile.EFFECT.NONE) ret += "\n" + pc.tile.effect2;
        }

        return ret;
    }

    /// <summary>
    /// Tell if this entity is immune to a certain status effect
    /// </summary>
    /// <param name="status">The status effect in question</param>
    /// <returns></returns>
    public bool IsImmune(string status)
    {
        return user.immunities.Contains(status);
    }

}
