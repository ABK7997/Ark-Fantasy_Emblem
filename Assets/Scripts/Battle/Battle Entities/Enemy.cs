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
    }

    /// <summary>
    /// Method describing how enemies will behave when it's their turn
    /// </summary>
    public virtual void Behavior()
    {
        string command = "ATTACK";
        Entity target = GetRandomPlayer();

        party.ExecuteAction(command, this, target);
        ResetTimer();
    }

    //Get a random player from the opposite party to target
    protected Entity GetRandomPlayer()
    {
        List<Entity> targets = party.GetLivingEnemies();

        int selection = Random.Range(0, targets.Count);

        //Obscure Target
        if (targets.Count > 1 && targets[selection].CheckEffect("OBSCURE"))
        {
            return ObscureEffect(targets, targets[selection]);
        }

        //Normal Selection
        else return targets[selection]; 
    }

    //Get a random ally from this one's enemy party to target
    protected Entity GetRandomAlly()
    {
        int selection = Random.Range(0, party.party.Count);

        return party.party[selection];
    }

    //EFFECT - OBSCURE
    protected Entity ObscureEffect(List<Entity> targets, Entity previousTarget)
    {
        int chance = 70;

        //Change target
        if (Random.Range(0, 100) <= chance)
        {
            targets.Remove(previousTarget);

            int selection = Random.Range(0, targets.Count);

            targets.Add(previousTarget);

            return targets[selection];
        }

        //Chance failed - same target acquired
        else return previousTarget;
    }
}
