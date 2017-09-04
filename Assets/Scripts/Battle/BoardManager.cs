using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Manages the grid on which all battles take place
/// </summary>
public class BoardManager : MonoBehaviour {

    //Board - General
    /// <summary>The Main Camera</summary>
    public Camera cam;

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

    /// <summary>A variable which helps with sizing the board properly</summary>
    public int scaling;

    /// <summary>
    /// The tile set which is chosen from at random to create the board
    /// </summary>
    public GameObject[] tiles;

    private List<Vector3> gridPositions = new List<Vector3>(); //A precise layout of coordinates set up in advance for placing tiles
    private Transform boardHolder; //Transform generated in BoardSetup() which holds all the tiles as children of one GameObject 

    private void Awake()
    {
        StartingCoords();
    }

    //Runs InitializeGrid() and BoardSetup() functions to set the stage for battle
    void Start()
    {
        cam.transform.position = new Vector3(2.5f * (columns - 1), 2.5f * (rows - 1), -1); //Center camera
        cam.orthographicSize = scaling + (columns * 1.75f);

        rows *= scaling;
        columns *= scaling;

        InitializeGrid();
        BoardSetup();
    }

    //Sets up tile locations for each (x, y) coordinate on the grid based on the columns and rows sizes
    void InitializeGrid()
    {
        for (int x = 0; x < columns; x+=scaling)
        {
            for (int y = 0; y < rows; y+=scaling) gridPositions.Add(new Vector3(x, y)); //No z-coordinate
        }
    }

    //Instantiates random tiles at each (x, y) coordinate created by InitializeGrid()
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = 0; x < columns; x += scaling)
        {
            for (int y = 0; y < rows; y += scaling)
            {
                GameObject toInstantiate = tiles[Random.Range(0, tiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    void StartingCoords()
    {
        foreach (Vector2 v in playerCoordinates)
        {
            v.Set(v.x * scaling, v.y * scaling);
        }

        foreach (Vector2 v in enemyCoordinates)
        {
            v.Set(v.x * scaling, v.y * scaling);
        }
    }
}
