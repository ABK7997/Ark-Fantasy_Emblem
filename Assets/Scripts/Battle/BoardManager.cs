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

    public int scaling;

    //Data pulled from BoardInfo class
    private int columns, rows;
    [HideInInspector] public Vector2[] playerCoordinates;
    [HideInInspector] public Vector2[] enemyCoordinates;

    private GameObject[] tiles;
    private Tile[,] board;
    private List<Tile> boardList;

    //Inherent
    private List<Vector3> gridPositions = new List<Vector3>(); //A precise layout of coordinates set up in advance for placing tiles
    private Transform boardHolder; //Transform generated in BoardSetup() which holds all the tiles as children of one GameObject 

    /// <summary>
    /// Runs InitializeGrid() and BoardSetup() functions to set the stage for battle
    /// </summary>
    /// <param name="bi">The board information from the enemy party encountered</param>
    public void BoardInit(BoardInfo bi)
    {
        //Information from BoardInfo class
        playerCoordinates = bi.playerCoordinates;
        enemyCoordinates = bi.enemyCoordinates;
        columns = bi.columns;
        rows = bi.rows;
        tiles = bi.tiles;
        board = new Tile[rows, columns];

        cam.transform.position = new Vector3(2.5f * (columns - 1), 2.5f * (rows - 1), -1); //Center camera
        cam.orthographicSize = scaling + (rows * 1.75f);

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
        boardList = new List<Tile>();

        for (int x = 0; x < columns; x += scaling)
        {
            for (int y = 0; y < rows; y += scaling)
            {
                GameObject toInstantiate = tiles[Random.Range(0, tiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);

                board[y / scaling, x / scaling] = instance.GetComponent<Tile>();
                boardList.Add(instance.GetComponent<Tile>());

                
            }
        }
    }

    /// <summary>
    /// Get a tile on the board at a certain position
    /// </summary>
    /// <param name="position">The position to search at</param>
    /// <returns>The tile at the given position (if it exists)</returns>
    public Tile GetTile(Vector3 position)
    {
        if (position.x / scaling < 0) return null;
        if (position.x / scaling > columns/scaling) return null;

        if (position.y / scaling < 0) return null;
        if (position.y / scaling > rows/scaling) return null;

        try
        {
            return board[(int)position.y / scaling, (int)position.x / scaling];
        } catch (System.Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// Cycle through every tile on the board to get which tile the mouse is hovering over
    /// </summary>
    /// <returns>The tile the player is selecting</returns>
    public Tile GetHoveringTile(Entity mover, List<Entity> party1, List<Entity> party2)
    {
        Vector3 position = mover.transform.position;

        //Tiles adjacent to mover
        List<Tile> moveSpaces = new List<Tile>();

        Tile right = GetTile(new Vector3(position.x + scaling, position.y));
        Tile left = GetTile(new Vector3(position.x - scaling, position.y));
        Tile up = GetTile(new Vector3(position.x, position.y + scaling));
        Tile down = GetTile(new Vector3(position.x, position.y - scaling));

        if (right != null) moveSpaces.Add(right);
        if (left != null) moveSpaces.Add(left);
        if (up != null) moveSpaces.Add(up);
        if (down != null) moveSpaces.Add(down);

        //Disqualify tiles that are occupied by other entities
        foreach (Entity e in party1)
        {
            Tile t = e.pc.GetTile();

            if (moveSpaces.Contains(t))
            {
                moveSpaces.Remove(t);
            }
        }
        foreach (Entity e in party2)
        {
            Tile t = e.pc.GetTile();

            if (moveSpaces.Contains(t))
            {
                moveSpaces.Remove(t);
            }
        }

        foreach (Tile t in boardList)
        {
            if (t.Hovering && moveSpaces.Contains(t)) return t;
        }

        return null;
    }
}
