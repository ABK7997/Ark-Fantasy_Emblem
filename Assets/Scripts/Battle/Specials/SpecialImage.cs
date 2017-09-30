using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialImage : MonoBehaviour {

    /// <summary>
    /// Text for the name of the special
    /// </summary>
    public Text nameText;

    /// <summary>
    /// Test for the HP or Turn cost of the special
    /// </summary>
    public Text costText;

    /// <summary>
    /// The sprite or image representing this special
    /// </summary>
    public Image img;

    /// <summary>
    /// Change this image's sprite
    /// </summary>
    /// <param name="sprite">image to be changed to</param>
    public void SetSprite(Sprite sprite)
    {
        img.sprite = sprite;
    }

    /// <summary>
    /// Change this image's special title
    /// </summary>
    /// <param name="specialName">the text to be changed to</param>
    public void SetName(string specialName)
    {
        nameText.text = specialName;
    }

    /// <summary>
    /// Change this image's cost. Can be switched between Skill, Tech, and Spell costs
    /// </summary>
    /// <param name="cost">the text to be changed to</param>
    public void SetCost(string cost)
    {
        costText.text = cost;
    }

}
