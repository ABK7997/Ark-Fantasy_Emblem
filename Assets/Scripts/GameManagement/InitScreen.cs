using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A short script for creating all the permanent objects in the game (such as the player party) d
/// </summary>
public class InitScreen : Manager {

    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        LoadScene("main_menu");
	}
}
