using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect {

    private string effectName;

    private int turnTimer;

    /// <summary>
    /// Constructor for a status effect
    /// </summary>
    /// <param name="effName">Name of the effect</param>
    /// <param name="turns">The number of user turns this effect lasts</param>
    public Effect(string effName, int turns)
    {
        EffectName = effName;
        turnTimer = turns;
    }

    /// <summary>
    /// Counts down by one the number of turns an effect will continue to last
    /// </summary>
    public void Turn()
    {
        TurnTimer--;
    }

    /// <summary>
    /// The name of this status effect
    /// </summary>
    public string EffectName
    {
        get
        {
            return effectName;
        }
        set
        {
            effectName = value;
        }
    }

    /// <summary>
    /// The number of turns this effect will last
    /// </summary>
    public int TurnTimer
    {
        get
        {
            return turnTimer;
        }
        set
        {
            turnTimer = value;
        }
    }
}
