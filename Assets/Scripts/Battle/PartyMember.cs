using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Subclass of Entity. PartyMembers are player-controlled characters
/// </summary>
public class PartyMember : Entity {

    /// <summary>The image on the character's stat canvas which shows his action bar charging</summary>
    public Image speedBar;

    //Calls base; sets the speed bar to 0
    protected override void Start()
    {
        base.Start();

        speedBar.rectTransform.sizeDelta = new Vector2(0, barsHeight);

        //moveTimer += Random.Range(0.000f, 50.000f);
    }

    /// <summary>
    /// Calls base; Manages the increasing speed bar until full
    /// </summary>
    public override void UpdateTime()
    {
        base.UpdateTime();

        //Update Speed Bar Progress
        float ratio = moveTimer / 100f;
        speedBar.rectTransform.sizeDelta = new Vector2(barsLength * ratio, barsHeight);
    }

}
