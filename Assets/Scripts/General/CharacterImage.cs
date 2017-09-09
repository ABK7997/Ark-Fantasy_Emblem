using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CharacterImage : MonoBehaviour, IPointerEnterHandler {

    /// <summary>
    /// The name of the character this image represents
    /// </summary>
    public string nameOfChar;

    /// <summary>
    /// The image rendering over the character selection button
    /// </summary>
    public Image charImage;

    /// <summary>
    /// The BattlePrep script which contains all selector images
    /// </summary>
    public BattlePrep bp;

    private bool hovering = false;

    //The character this image represents
    public PartyMember character;

    /// <summary>
    /// Changes color of the character selection images to show who has been chosen so far
    /// </summary>
	public void ChangeColor()
    {
        //Select
        if (charImage.color == Color.white)
        {
            if (bp.IsFull()) return; //Can't add to a full party

            charImage.color = Color.black;
            bp.AddToParty(character);
        }

        //Deselect
        else
        {
            charImage.color = Color.white;
            bp.RemoveFromParty(character);
        }
    }

    /// <summary>
    /// Setter method for the PartyMember this script holds
    /// </summary>
    /// <param name="p">The PartyMember to be associated with this script</param>
    public void SetCharacter(PartyMember p)
    {
        character = p;
    }

    /// <summary>
    /// Get the party member associated with this script
    /// </summary>
    /// <returns>A PartyMember</returns>
    public PartyMember GetCharacter()
    {
        return character;
    }

    /// <summary>
    /// Tells if the player has their mouse over this particular party image
    /// </summary>
    /// <returns>hovering - the mouse is/isn't over the member image</returns>
    public bool IsHovering()
    {
        return hovering;
    }

    /// <summary>
    /// Mouse is over the image
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        //throw new NotImplementedException();
    }

    /// <summary>
    /// Mouse exits the image
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
