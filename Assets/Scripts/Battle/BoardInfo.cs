using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which accompanies all instances of Enemy Avatar. 
/// Contains battle board data, such as offering differing landscapes and starting coordinates for more unique battles
/// </summary>
public class BoardInfo : MonoBehaviour {

    public int enemyLevel = 1;

    /// <summary>X and Y sizese respectively</summary>
    public int columns, rows;

    /// <summary>
    /// Places where player party members will start on the board
    /// </summary>
    public Vector2[] playerCoordinates;

    /// <summary>
    /// Places where the enemy party members will start on the board
    /// </summary>
    public Vector2[] enemyCoordinates;

    /// <summary>
    /// The tile set which is chosen from at random to create the board
    /// </summary>
    public GameObject[] tiles;
}
