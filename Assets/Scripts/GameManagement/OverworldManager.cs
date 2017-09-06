using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldManager : Manager {

    public Overworld overworld;

    protected override void Awake()
    {
        base.Awake();

        //Instantiate overworld
        if (CheckSingle<Overworld>() == 1)
        {
            Debug.Log(SceneManager.GetActiveScene().name + ": Area Manager did not yet exist, so it was created.");
            Instantiate(overworld);
        }
    }
}
