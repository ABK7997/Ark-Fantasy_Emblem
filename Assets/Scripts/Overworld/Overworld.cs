using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld : MonoBehaviour
{
    /// <summary>
    /// The party the player has just collided with and will then battle
    /// </summary>
    [HideInInspector] public EnemyAvatar encounteredParty;

    /// <summary>
    /// Collection of all enemy parties represented as avatars in the overworld
    /// </summary>
    public List<EnemyAvatar> activeEnemies;

    public GameObject enemyHolder;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Disable most overworld information during battle or enable it again when transitioning back to the Overworld scene
    /// </summary>
    /// <param name="b">set on or off</param>
    public void Activate(bool b)
    {
        foreach (EnemyAvatar e in activeEnemies)
        {
            e.SetVisible(b);
        }
    }
}
