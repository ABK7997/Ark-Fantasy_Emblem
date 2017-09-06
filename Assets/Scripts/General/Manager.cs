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
        //Instantiate first game manager
        if (CheckSingle<GameManager>() == 1)
        {
            Debug.Log(SceneManager.GetActiveScene().name + ": Game Manager did not yet exist, so it was created.");
            Instantiate(Resources.Load("Game Manager"));
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

    //Check if any singleton has been instantiated or not.
    //0 - more than 1 singleton. Serious error
    //1 - no singleton exists
    //-1 - a singleton is already in existence
    protected int CheckSingle<T>() where T : MonoBehaviour
    {
        T singleton = FindObjectOfType<T>();
        string sceneName = SceneManager.GetActiveScene().name;

        //If there is more than 1 singleton (which would be an ERROR)
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Debug.LogError("Scene: " + sceneName + ": [Singleton] Something went really wrong " +
                " - there should never be more than 1 singleton!" +
                " Reopening the scene might fix it.");

            return 0;
        }

        //Object does not exist; instantiate new one
        else if (singleton == null)
        {
            return 1;
        }

        //Object already exists
        else
        {
            Debug.Log(singleton.name + " already exists.");
            return -1;
        }
    }
}
