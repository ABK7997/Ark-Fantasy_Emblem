using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    /// <summary>
    /// The name of this tile
    /// </summary>
    public string tileName;

    private bool hovering = false;

    /// <summary>
    /// The types of status effects a tile can have on a player
    /// </summary>
    public enum EFFECT
    {
        NONE, SOGGY, GROUNDED, STUCK, OBSCURED, RECOVERY, HIDDEN, COVER, FORTIFIED, HAZARD
    }
    public EFFECT effect1, effect2;

    private void OnMouseOver()
    {
        hovering = true;
    }

    private void OnMouseExit()
    {
        hovering = false;
    }

    /// <summary>
    /// Get and set if the mouse is hovering over a tile
    /// </summary>
    public bool Hovering
    {
        get
        {
            return hovering;
        }
        set
        {
            hovering = value;
        }
    }
}
