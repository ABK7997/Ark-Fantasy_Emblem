using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to help control the game state
/// </summary>
public class GameManager : MonoBehaviour {

    protected GameManager() { }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
