using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Logging : MonoBehaviour
{
    //turn these on and off in inspector to toggle logging various subsystems (ALWAYS_LOG is...always logged)
    //add more as desired
    public bool LogCore;
    public bool LogBeatmap;
    public bool LogUI;

    public enum Category
    {
        /// <summary>
        /// Always logged
        /// </summary>
        ALWAYS_LOG = 0,
        /// <summary>
        /// Systems necessary to run the game (databases, unity scene stuff, etc...)
        /// </summary>
        CORE = 1,
        /// <summary>
        /// Beat map interpretation system
        /// </summary>
        SONG_DATA = 2,
        /// <summary>
        /// User interface system
        /// </summary>
        UI = 3,
        
        //add more as desired
    }
    public enum Level
    {
        LOG = 0,
        LOG_WARNING = 1,
        LOG_ERROR = 2,
    }

    private bool WillLogCategory(Category category)
    {
        //NOTE: Unity will not compile 'switch' expressions, only use block-form.
        //add more as desired
        switch (category)
        {
            case Category.ALWAYS_LOG: return true;
            case Category.CORE: return LogCore;
            case Category.SONG_DATA: return LogBeatmap;
            case Category.UI: return LogUI;
            default:
                return false;
        }
    }
    public void Log(Category category, string message, Level level)
    {
        //this method is blank once the game is built (only works in editor)
#if UNITY_EDITOR

        //if this category is not logged, bounce outie
        if (!WillLogCategory(category))
            return;

        //log the message as the specified Debug.Log level
        //NOTE: Unity will not compile 'switch' expressions, only use block-form.
        switch (level)
        {
            case Level.LOG:
                Debug.Log(message); break;
            case Level.LOG_WARNING:
                Debug.LogWarning(message); break;
            case Level.LOG_ERROR:
                Debug.LogError(message); break;
            default: break;
        }
#endif
    }
    public void LogFormat(Category category, string format, Level level, params object[] args)
    {
        //this method is blank once the game is built (only works in editor)
#if UNITY_EDITOR

        //if this category is not logged, bounce outie
        if (!WillLogCategory(category))
            return;

        //log the message as the specified Debug.Log level
        //NOTE: Unity will not compile 'switch' expressions, only use block-form.
        switch (level)
        {
            case Level.LOG:
                Debug.LogFormat(format, args); break;
            case Level.LOG_WARNING:
                Debug.LogWarningFormat(format, args); break;
            case Level.LOG_ERROR:
                Debug.LogErrorFormat(format, args); break;
            default: break;
        }
#endif
    }
}
