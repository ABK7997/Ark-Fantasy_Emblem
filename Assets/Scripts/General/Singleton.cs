using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A superclass which can be used to determine that certain objects will have
/// a single, continuous instance throughout the game - NOT currently in use; exists primarily for reference
/// </summary>
/// <typeparam name="T">The type of object which will be made permanent and unique</typeparam>
public class Singleton<T> : MonoBehaviour where T: MonoBehaviour {

    private static T _instance;

    private static object _lock = new object();

    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting) //When quitting
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock(_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T)); //Search for existing object of type T

                    string sceneName = SceneManager.GetActiveScene().name;

                    //If there is more than 1 singleton (which would be an ERROR)
                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("Scene: " + sceneName + ": [Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }

                    //Singleton does NOT exist in scene; FIRST INSTANCE needs to be created
                    if (_instance == null)
                    {
                        /*
                        GameObject singleton = new GameObject(); //create original singleton
                        _instance = singleton.AddComponent<T>(); //add specific type to singleton
                        singleton.name = typeof(T).ToString() + " (singleton)";

                        DontDestroyOnLoad(singleton); //Will now persist for the rest of the session

                        Debug.Log("Scene: " + sceneName + ": [Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                                */

                        Instantiate(Resources.Load("Game Manager"));
                    }

                    //Singleton DOES exist in scene; new one is not added
                    else
                    {
                        Debug.Log("Scene: " + sceneName + ": [Singleton] Object already exists");
                    }
                }

                return _instance;
            }
        }
    }

	public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
