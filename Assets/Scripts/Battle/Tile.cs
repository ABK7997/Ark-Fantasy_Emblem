using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    /// <summary>
    /// The name of this tile
    /// </summary>
    public string tileName;

    private bool occupied = false;

    /// <summary>
    /// The types of status effects a tile can have on a player
    /// </summary>
    public enum EFFECT
    {
        NONE, SOGGY, GROUNDED, STUCK, OBSCURED, RECOVERY, HIDDEN, COVER, FORTIFIED
    }
    public EFFECT effect1, effect2;

    /// <summary>
    /// Getter and setter for if this title is occupied by an entity
    /// </summary>
    public bool Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }
}
