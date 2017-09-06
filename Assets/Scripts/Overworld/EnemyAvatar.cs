using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvatar : Avatar {

    /// <summary>
    /// The Board information attached to this particular enemy party
    /// </summary>
    public BoardInfo bi;

    /// <summary>
    /// The overworld the enemy exists in
    /// </summary>
    public Overworld ow;

    /// <summary>
    /// The enemy party makeup, usually unique between avatars
    /// </summary>
    public List<Entity> party;

    /// <summary>
    /// Method necessary for populating the EnemyParty in the battle scene
    /// </summary>
    /// <returns>party - this script's attached enemies</returns>
    public List<Entity> GetParty()
    {
        return party;
    }

    /// <summary>
    /// Disable or enable the avatar sprite
    /// </summary>
    public void SetVisible(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
    }

    void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.gameObject.tag == "Player")
        {
            ow.encounteredParty = this;
        }
    }
}
