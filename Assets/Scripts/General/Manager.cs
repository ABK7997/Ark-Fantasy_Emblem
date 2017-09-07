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

    /// <summary>
    /// Load any scene from any other scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to be loaded</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Exits game to desktop
    /// </summary>
    //Quit game
    public void Quit()
    {
        Application.Quit();
    }

    public void DetroySingletons()
    {
        Destroy(FindObjectOfType<Overworld>().gameObject);
    }

    //Check if any singleton has been instantiated or not.
    //0 - more than 1 singleton. Serious error
    //1 - no singleton exists
    //-1 - a singleton is already in existence
    protected int CheckSingle<T>() where T : MonoBehaviour
    {
        T singleton = FindObjectOfType<T>();
        string sceneName = SceneManager.GetActiveScene().name;
        string sig = "Scene: " + sceneName + ": [Singleton] ";

        //If there is more than 1 singleton (which would be an ERROR)
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Debug.LogError(sig + singleton.name + " Something went really wrong " +
                " - there should never be more than 1 singleton!" +
                " Reopening the scene might fix it.");

            return 0;
        }

        //Object does not exist; instantiate new one
        else if (singleton == null)
        {
            Debug.Log(sig + "Object does not exist yet.");

            return 1;
        }

        //Object already exists
        else
        {
            Debug.Log(sig + singleton.name + " already exists.");
            return -1;
        }
    }
}
