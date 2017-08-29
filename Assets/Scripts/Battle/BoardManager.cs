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
    /// The tile set which is chosen from at random to create the board
    /// </summary>
    public GameObject[] tiles;

    private List<Vector3> gridPositions = new List<Vector3>(); //A precise layout of coordinates set up in advance for placing tiles
    private Transform boardHolder; //Transform generated in BoardSetup() which holds all the tiles as children of one GameObject 

    //Runs InitializeGrid() and BoardSetup() functions to set the stage for battle
    void Start()
    {
        InitializeGrid();
        BoardSetup();

        cam.transform.position = new Vector3(columns / 2, rows / 2, -1); //Center camera
    }

    //Sets up tile locations for each (x, y) coordinate on the grid based on the columns and rows sizes
    void InitializeGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++) gridPositions.Add(new Vector3(x, y)); //No z-coordinate
        }
    }

    //Instantiates random tiles at each (x, y) coordinate created by InitializeGrid()
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject toInstantiate = tiles[Random.Range(0, tiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }
}
