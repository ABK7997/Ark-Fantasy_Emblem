using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    private string spellName;

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
    /// The nature of this spell
    /// </summary>
    public enum TYPE {
        ATTACK, HEAL, AILMENT
    }
    public TYPE type;

    //Componenets
    private SpriteRenderer render;

    private void Start()
    {
        spellName = gameObject.name;
        render = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Put together all information about the spell
    /// </summary>
    /// <returns>A string of all relevant spell stats</returns>
    public string DisplayStats()
    {
        return spellName + "\n" +
            cost + " HP\n";
    }

    public Sprite getSprite()
    {
        return render.sprite;
    }
}
