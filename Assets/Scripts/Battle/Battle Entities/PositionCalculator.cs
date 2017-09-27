using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCalculator {

    private Entity user;
    private Vector3 originalPosition;

    private SpriteRenderer render;

    //Tiles
    public Tile tile, tileProspect;

    public PositionCalculator(Entity u, SpriteRenderer r)
    {
        user = u;

        render = r;
    }

    /// <summary>
    /// Move entity to temporary position on the battlefield
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public void SetPosition(float x, float y)
    {
        user.transform.position = new Vector3(user.transform.position.x + x, user.transform.position.y + y, 0);
    }

    /// <summary>
    /// Move the entity to a new permanent location on the board
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ChangeLocation(float x, float y)
    {
        user.transform.position = new Vector3(x, y, 1);
    }

    /// <summary>
    /// Reset entity to original position
    /// </summary>
    public void ResetPosition()
    {
        user.transform.position = originalPosition;
        if (render != null) render.color = Color.white;
    }

    /// <summary>
    /// Change this entity's reset position
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public void SetOriginalPosition(float x, float y)
    {
        originalPosition = new Vector3(x, y, 0);
    }

    /// <summary>
    /// Determines if the target entity is to the right of the user entity
    /// </summary>
    /// <param name="target">Target entity</param>
    /// <returns>True - target x is greater than user x; False - otherwise</returns>
    public bool IsRightOf(Entity target)
    {
        return user.transform.position.x < target.transform.position.x;
    }

    /// <summary>
    /// Determines if the target entity is to the left of the user entity
    /// </summary>
    /// <param name="target">Target entity</param>
    /// <returns></returns>
    public bool IsLeftOf(Entity target)
    {
        return user.transform.position.x > target.transform.position.x;
    }

    /// <summary>
    /// Flip entity if the target is on their opposite side
    /// </summary>
    /// <param name="target">Entity being targeted</param>
    public void FlipTowardsTarget(Entity target)
    {
        if (IsRightOf(target))
        {
            if (render.flipX) render.flipX = false;
            if (!target.GetRender().flipX) target.GetRender().flipX = true;
        }
        else if (IsLeftOf(target))
        {
            if (!render.flipX) render.flipX = true;
            if (target.GetRender().flipX) target.GetRender().flipX = false;
        }
    }

    /***TILES***/

    /// <summary>
    /// Set or change the tile this entity is standing on
    /// </summary>
    /// <param name="t">The new tile to be set</param>
    public void SetTile(Tile t)
    {
        tile = t;
        user.transform.position = tile.transform.position;
        SetOriginalPosition(user.transform.position.x, user.transform.position.y);
    }

    /// <summary>
    /// The tile this entity is standing on
    /// </summary>
    /// <returns>A board tile</returns>
    public Tile GetTile()
    {
        return tile;
    }

    /// <summary>
    /// The tile this entity might move to if the player decides to
    /// </summary>
    /// <param name="t">The prospective tile</param>
    public void SetTileProspect(Tile t)
    {
        tileProspect = t;
    }

    /// <summary>
    /// Change this entity's position and tile on the board
    /// </summary>
    public void Move()
    {
        tile = tileProspect;
        tileProspect = null;

        user.transform.position = new Vector3(tile.transform.localPosition.x, tile.transform.localPosition.y, 0);
        SetOriginalPosition(user.transform.position.x, user.transform.position.y);

        user.ResetTimer();
    }

    /// <summary>
    /// Get the first status effect of this entity's occupied tile
    /// </summary>
    /// <returns>The status effect of the tile</returns>
    public Tile.EFFECT GetTileEffect1()
    {
        return tile.effect1;
    }

    /// <summary>
    /// Get the second status effect of this entity's occupied tile. Many tiles do not have a second effect
    /// </summary>
    /// <returns>The status effect of the tile</returns>
    public Tile.EFFECT GetTileEffect2()
    {
        return tile.effect2;
    }
}
