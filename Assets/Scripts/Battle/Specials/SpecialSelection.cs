using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSelection : MonoBehaviour {

    /// <summary>
    /// Content container for specials buttons
    /// </summary>
    public RectTransform content;

    /// <summary>
    /// Currently available specials gotten from party members
    /// </summary>
    public List<SpecialImage> choices;

    /// <summary>
    /// Fill the special selection bar with Skills, Techs, or Spells
    /// </summary>
    /// <param name="specials">list of special abilities</param>
	public void SetSpecials(List<Special> specials)
    {
        int containersUsed = 0;

        for (int i = 0; i < specials.Count; i++, containersUsed++)
        {
            choices[i].gameObject.SetActive(true);
            choices[i].SetName(specials[i].GetName());
            choices[i].SetCost(specials[i].GetCost());
            choices[i].SetSprite(specials[i].GetSprite());
        }

        //Disable unused images
        for (int i = specials.Count; i < choices.Count; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        content.sizeDelta = new Vector2(containersUsed * 100, content.sizeDelta.y);
    }

    /// <summary>
    /// Fill the speical selection bar with the party's inventory
    /// </summary>
    /// <param name="items">list of available items</param>
    public void SetItems(List<Item> items)
    {
        int containersUsed = 0;

        for (int i = 0; i < items.Count; i++, containersUsed++)
        {
            choices[i].gameObject.SetActive(true);
            choices[i].SetName(items[i].itemName);
            choices[i].SetCost(items[i].stock + "");
            choices[i].SetSprite(items[i].GetSprite());
        }

        //Disable unused images
        for (int i = items.Count; i < choices.Count; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        content.sizeDelta = new Vector2(containersUsed * 100, content.sizeDelta.y);
    }
}
