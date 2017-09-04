using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A superclass which provides very common methods to Menus and such
/// </summary>
public class Manager : MonoBehaviour {

    protected virtual void Awake()
    {
        GameManager gm = FindObjectOfType<GameManager>(); //Search for existing object of type T
        string sceneName = SceneManager.GetActiveScene().name;

        //If there is more than 1 singleton (which would be an ERROR)
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Debug.LogError("Scene: " + sceneName + ": [Singleton] Something went really wrong " +
                " - there should never be more than 1 singleton!" +
                " Reopening the scene might fix it.");
        }

        //Object does not exist; instantiate new one
        else if (gm == null)
        {
            Instantiate(Resources.Load("Game Manager"));
            Debug.Log("Game Manager created");
        }

        //Object already exists
        else
        {
            Debug.Log("Game Manager already exists");
        }
    }

    //Change scene
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Quit game
    public void Quit()
    {
        Application.Quit();
    }
}
