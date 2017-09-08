using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSelection : MonoBehaviour {

    /// <summary>
    /// Currently available specials gotten from party members
    /// </summary>
    public List<SpecialImage> choices;

	public void SetSpecials(List<Special> specials)
    {
        for (int i = 0; i < specials.Count; i++)
        {
            choices[i].SetName(specials[i].GetName());
            choices[i].SetCost(specials[i].GetCost());
            choices[i].SetSprite(specials[i].GetSprite());
        }
    }
}
