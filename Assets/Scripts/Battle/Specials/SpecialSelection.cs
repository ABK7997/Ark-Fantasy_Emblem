using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSelection : MonoBehaviour {

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
        for (int i = 0; i < specials.Count; i++)
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
    }

    /// <summary>
    /// Fill the speical selection bar with the party's inventory
    /// </summary>
    /// <param name="items">list of available items</param>
    public void SetItems(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
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
    }
}
