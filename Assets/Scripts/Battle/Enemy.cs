using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Subclass of Entity. Enemies are always in the opposite party fighting the player
/// </summary>
public class Enemy : Entity {

    //Calls base; Starts the enemy's speed bar at a random point between 0% and 50% full
    protected override void Start()
    {
        base.Start();

        moveTimer += Random.Range(0.000f, 50.000f);
    }

    /// <summary>
    /// Calls base; Enemy takes a turn when their speed bar is ready
    /// </summary>
    public override void UpdateTime()
    {
        base.UpdateTime();

        if (ready)
        {
            behavior();
            resetTimer();
        }
    }

    //Method describing how enemies will behave when it's their turn
    protected virtual void behavior()
    {
        string command = "ATTACK";
        Entity target = getRandomPlayer();

        party.executeAction(command, this, target);
    }

    //Get a random player from the opposite party to target
    protected Entity getRandomPlayer()
    {
        int selection = Random.Range(0, party.oppositeParty.Count);

        return party.oppositeParty[selection];
    }

    //Get a random ally from this one's enemy party to target
    protected Entity getRandomAlly()
    {
        int selection = Random.Range(0, party.party.Count);

        return party.party[selection];
    }
}
