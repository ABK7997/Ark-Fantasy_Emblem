using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCalculator {

    Entity user;
    BattleCalculator bc;

    public EffectCalculator(Entity u, BattleCalculator calc)
    {
        user = u;
        bc = calc;
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
}
