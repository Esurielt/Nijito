using System.Collections.Generic;
using UnityEngine;

public static class DatabaseManager
{
    private const string PATH_AFTER_RESOURCE_DIR = "Databases/";

    private static readonly List<DatabaseHelper> _databaseHelpers = new List<DatabaseHelper>();

    public static bool Initialize()
    {
        if (_databaseHelpers.Count == 0)
        {
            //register databases
            RegisterDB<SongData.ChannelValueInfo>("ChannelValueInfos", "Beatmap Channel Value Info");

            return true;
        }
        else
        {
            Game.Log(Logging.Category.CORE, "Duplicate call to DatabaseManager.Initialize(). This should only be called once.", Logging.Level.LOG_WARNING);
            return false;
        }
    }
    private static bool RegisterDB<DBType>(string directoryName, string loggingName) where DBType : ScriptableObject
    {
        var db = new DatabaseHelper<DBType>(PATH_AFTER_RESOURCE_DIR + directoryName, loggingName);
        if (!db.Load())
        {
            return false;
        }
        _databaseHelpers.Add(db);
        return true;
    }
    public static DatabaseHelper<DBType> GetDB<DBType>() where DBType : ScriptableObject
    {
        var found = _databaseHelpers.Find(db => db.ManagedType == typeof(DBType));
        return found as DatabaseHelper<DBType>;
    }
}
