using MessageBoxes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SongData;

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
    public static bool DatabaseInitialized { get; private set; }

    public Canvas PersistentCanvas;
    public RectTransform MessageBoxContainer;

    //called before Start() on game initialization. Order of Awake() calls can not be guaranteed.
    private void Awake()
    {
        //if singleton has already been assigned
        if (Self != null)
        {
            Debug.Log("Deactivating duplicate instance of Game.");
            Destroy(this.gameObject);
            return;
        }

        Debug.Log("Assigning Game.Self singleton instance.");

        //this becomes the singleton
        Self = this;

        //keep this object around when scenes change (also keeps persistent canvas)
        DontDestroyOnLoad(gameObject);

        //get component references
        _logging = GetComponent<Logging>();
        _messageBoxManager = GetComponent<MessageBoxManager>();
        _audioSource = GetComponent<AudioSource>();
    }

    //public static access (static global-access systems like logging, databases, etc...)
    public static void Log(Logging.Category category, string message, Logging.Level level = Logging.Level.LOG)
    {
        if (Self != null && Self._logging != null)
        {
            Self._logging.Log(category, message, level);
        }
    }
    public static void LogFormat(Logging.Category category, string format, Logging.Level level = Logging.Level.LOG, params object[] args)
    {
        if (Self != null && Self._logging != null)
        {
            Self._logging.LogFormat(category, format, level, args);
        }
    }
    public static void MessageBox(string title, string message, params ButtonTemplate[] menuButtons)
    {
        MessageBox(new MessageTemplate(title, message, menuButtons));
    }
    public static void MessageBox(MessageTemplate messageTemplate)
    {
        Self._messageBoxManager.MessageBox(messageTemplate);
    }
    public static AudioSource AudioSource => Self._audioSource;

    //private Game component references (owned by singleton)
    private Logging _logging;
    private MessageBoxManager _messageBoxManager;
    private AudioSource _audioSource;

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
            DatabaseInitialized = true;
        }

        //pass control to next system
        OpenEditorMainMenu();
    }
    public static void LoadScene(string sceneName, System.Action onLoadComplete = null)
    {
        Self.StartCoroutine(LoadSceneCoroutine(sceneName, onLoadComplete));
    }
    private static IEnumerator LoadSceneCoroutine(string sceneName, System.Action onLoadComplete = null)
    {
        var async = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => async.isDone);
        LogFormat(Logging.Category.CORE, "New Scene Loaded: {0}", Logging.Level.LOG, SceneManager.GetActiveScene().name);

        onLoadComplete?.Invoke();
    }
    public static void OpenEditorMainMenu()
    {
        LoadScene(SceneNames.EDITOR_MENU);
    }
    public static void OpenBeatmapEditor(string songName, BeatmapDifficulty difficulty, SongData.Beatmap fromBeatmap = null)
    {
        LoadScene(SceneNames.PIANO_ROLL_EDITOR, () => OnBeatmapEditorLoaded(songName, difficulty, fromBeatmap));
    }
    private static void OnBeatmapEditorLoaded(string songName, BeatmapDifficulty difficulty, SongData.Beatmap fromBeatmap = null)
    {
        var editor = FindObjectOfType<Editors.BeatmapEditor.BeatmapEditor>();
        if (editor != null)
        {
            editor.Initialize(songName, difficulty, fromBeatmap);
        }
        else
        {
            Log(Logging.Category.SONG_DATA, "Unable to locate beatmap editor component in scene.", Logging.Level.LOG_ERROR);
        }
    }
    public static void OpenSongEditor(string songName)
    {
        LoadScene(SceneNames.SONG_METADATA_EDITOR, () => OnSongEditorLoaded(songName));
    }
    private static void OnSongEditorLoaded(string songName)
    {
        var editor = FindObjectOfType<Editors.SongMetadataEditor.SongMetadataEditor>();
        if (editor != null)
        {
            editor.Initialize(songName);
        }
        else
        {
            Log(Logging.Category.SONG_DATA, "Unable to locate song editor component in scene.", Logging.Level.LOG_ERROR);
        }
    }

    //void Update()
    //{
    //    //NOTE: Avoid using Update function wherever possible
    //}
}