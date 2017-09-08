using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special : MonoBehaviour {

    //Spell stats

    /// <summary>
    /// The multiplier effect the MAG stat has on this spell
    /// </summary>
    public int basePwr;

    /// <summary>
    /// The HP cost of this spell
    /// </summary>
    public int cost;

    /// <summary>
    /// A worded, lore description of this spell
    /// </summary>
    public string description;

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
        ATTACK, HEAL, REPAIR, AILMENT, BUFF
    }
    public TYPE type;

    //Componenets
    private SpriteRenderer render;

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
            case CLASS.SPELL: return cost + " HP";
            case CLASS.TECH: return cost + " Turns";
            
            default: return "";
        }
    }
}
