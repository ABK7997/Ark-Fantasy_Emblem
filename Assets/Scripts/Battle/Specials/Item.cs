using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public string itemName, description;
    public int weight, cost, stock;

    public enum EFFECT
    {
        HEAL, REVIVE
    }
    public EFFECT effect;

    private SpriteRenderer render;

    /// <summary>
    /// Get sprite associated with this item
    /// </summary>
    /// <returns>a sprite</returns>
    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    /// <summary>
    /// Consume this item and decrease stock
    /// </summary>
    /// <param name="target"></param>
    public void Use(Entity target)
    {
        switch (effect)
        {
            case EFFECT.HEAL: target.Hp += 15; break;

            case EFFECT.REVIVE:

                target.SetStatus("NORMAL");
                target.Hp += (int)(target.maxHP * 0.33);

                break;
        }

        stock--;
    }

}
