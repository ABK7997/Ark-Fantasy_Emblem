﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special : MonoBehaviour {

    //Spell stats

    /// <summary>
    /// The multiplier effect the MAG stat has on this spell
    /// </summary>
    public float basePwr;

    /// <summary>
    /// The HP cost of this spell
    /// </summary>
    public int cost;

    /// <summary>
    /// The length a status effect special lasts (in turns)
    /// </summary>
    public int turnTimer;

    /// <summary>
    /// A quantity which helps define a spell's chance of hitting
    /// </summary>
    public int baseAccuracy;

    /// <summary>
    /// A quantity which helps define a spell's chance of getting a critical hit
    /// </summary>
    public int baseCrit;

    /// <summary>
    /// A worded, lore description of this spell
    /// </summary>
    public string description;

    /// <summary>
    /// The time (in sec) this special takes to animate
    /// </summary>
    public float animationTime;

    /// <summary>
    /// The class of special
    /// </summary>
    public enum CLASS
    {
        SKILL, SPELL, TECH
    }
    public CLASS classification;

    /// <summary>
    /// The nature of this special within its class
    /// </summary>
    public enum TYPE
    {
        ATTACK, HEAL, REPAIR, EFFECT
    }
    public TYPE type;

    public enum EFFECT
    {
        NONE, OBSCURE
    }
    public EFFECT effect;

    //Componenets
    private SpriteRenderer render;

    //Animation

    /// <summary>
    /// An object which is instantiated to be the spell's animated form
    /// </summary>
    public AnimatedProjectile projectile;

    /// <summary>
    /// Put together all information about the spell
    /// </summary>
    /// <returns>A string of all relevant spell stats</returns>
    public string DisplayStats()
    {
        return gameObject.name + "\n" +
            cost + " HP\n";
    }

    /// <summary>
    /// Get sprite associated with this special move
    /// </summary>
    /// <returns>a sprite</returns>
    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    /// <summary>
    /// Get the name of this special ability
    /// </summary>
    /// <returns>the name of this special ability</returns>
    public string GetName()
    {
        return gameObject.name;
    }

    /// <summary>
    /// Get the HP or Turn cost of this special ability. Or nothing if there is no cost.
    /// </summary>
    /// <returns>a string detailing the appropriate cost of this special's class</returns>
    public string GetCost()
    {
        switch (classification)
        {
            case CLASS.SKILL: return turnTimer + " Turns";
            case CLASS.SPELL: return cost + " HP";
            case CLASS.TECH: return cost + " Recharge";
            
            default: return "";
        }
    }

    /// <summary>
    /// Create special as an animating game object
    /// </summary>
    /// <param name="user">The entity casting the Special</param>
    /// <param name="target">The target of the Special</param>
    public void StartAnimation(Entity user, Entity target, bool hit)
    {
        AnimatedProjectile p = Instantiate(projectile, user.transform.position, Quaternion.identity);

        p.StartAnimation(this, user, target, animationTime, hit);
    }

}