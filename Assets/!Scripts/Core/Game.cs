using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton game manager component. Manages core and backend functions and initialization data, delegating to other systems whenever possible. Attach one (and only one) of this MonoB to its own GameObject in each scene.
/// </summary>
[RequireComponent(typeof(Logging))]
public class Game : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of Game.
    /// </summary>
    public static Game Self;

    public Beatmap.Drummer.DrummerMapEditor TestingDrummerMapEditor;    //remove when no longer testing editor

    //called before Start() on game initialization. Order of Awake() calls can not be guaranteed.
    private void Awake()
    {
        //if singleton has already been assigned
        if (Self != null)
        {
            Debug.Log("Deactivating duplicate instance of Game.");
            gameObject.SetActive(false);
            return;
        }

        Debug.Log("Assigning Game.Self singleton instance.");

        //this becomes the singleton
        Self = this;

        //keep this object around when scenes change
        DontDestroyOnLoad(gameObject);

        //get component references
        _log = GetComponent<Logging>();
    }

    //public static access (static global-access systems like logging, databases, etc...)
    public static void Log(Logging.Category category, string message, Logging.Level level = Logging.Level.LOG)
    {
        if (Self != null && Self._log != null)
        {
            Self._log.Log(category, message, level);
        }
    }
    public static void LogFormat(Logging.Category category, string format, Logging.Level level = Logging.Level.LOG, params object[] args)
    {
        if (Self != null && Self._log != null)
        {
            Self._log.LogFormat(category, format, level, args);
        }
    }

    //private Game component references (owned by singleton)
    private Logging _log;

    //Order of Start() calls can not be guaranteed. Use this Start() method to call other core systems' init functions in appropriate order.
    void Start()
    {
        //game initialization goes here

        //load all databases (crash playmode if we fail to load one)
        if (!DatabaseManager.Initialize())
        {
            Log(Logging.Category.CORE, "Failed to load a database.", Logging.Level.LOG_ERROR);
        }
        else
        {
            Log(Logging.Category.CORE, "All databases loaded successfully.", Logging.Level.LOG);
        }

        //pass control to next system

        //below is for testing the beatmap editor, remove when done
        TestingDrummerMapEditor.Initialize(new Beatmap.Drummer.DrummerMapWriter(new Beatmap.Drummer.DrummerMap()), new Beatmap.BeatmapFileIOHelper_JSON("test"));
    }

    //void Update()
    //{
    //    //NOTE: Avoid using Update function wherever possible
    //}
}
