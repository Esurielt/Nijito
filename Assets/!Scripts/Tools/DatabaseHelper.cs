using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

public abstract class DatabaseHelper
{
    public string ResourcePath { get; protected set; }   //where below the resources folder are these assets located?
    public string LoggingName { get; protected set; }  //for debug log, what kind of assets are these?
    public abstract System.Type ManagedType { get; }

    public DatabaseHelper(string pathAfterResourceDirectory, string assetTypeNameForLogging)
    {
        ResourcePath = pathAfterResourceDirectory;
        LoggingName = assetTypeNameForLogging;
    }
    public abstract bool Load();

#if UNITY_EDITOR
    public static TObject GetAssetForCode<TObject>(string internalName, params string[] directoriesAfterResources) where TObject : ScriptableObject
    {
        string assetPath = "Assets/Resources/";
        foreach(var dir in directoriesAfterResources)
        {
            assetPath += dir + "/";
        }
        assetPath += internalName + ".asset";

        var loaded = AssetDatabase.LoadAssetAtPath<TObject>(assetPath);

        if (loaded == null)
            Debug.LogErrorFormat("Unable to retrieve asset at path \"{0}\". Have you moved or renamed the file? Try to stay organized, please.", assetPath);

        return loaded;
    }
    public abstract List<Object> GetAllAssetsForCode();
    public static List<TObject> GetAllAssetsOfTypeForCode<TObject>() where TObject : ScriptableObject
    {
        var guidsOfType = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(TObject).ToString()));
        if (guidsOfType.Length == 0) return new List<TObject>();

        var paths = guidsOfType.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
        return paths.Select(path => AssetDatabase.LoadAssetAtPath<TObject>(path)).ToList();
    }
#endif
}
public class DatabaseHelper<DBType> : DatabaseHelper where DBType : ScriptableObject
{
    //Helper class for managing and querying collections of ScriptableObjects (i.e. ModTemplates)

    public override System.Type ManagedType => typeof(DBType);

    protected static readonly string RESOURCES_DIRECTORY_PATH = Application.dataPath + "/Resources/";  //it's like a const, but not

    protected List<DBType> _database = new List<DBType>();      //actual database of assets

    //constructor
    public DatabaseHelper(string pathAfterResourceDirectory, string assetTypeNameForLogging)
        : base(pathAfterResourceDirectory, assetTypeNameForLogging) { }

    //methods
    public override bool Load()
    {
        //Load all assets of the specified type from the specified directory into a managed collection
        //return true if load was successful
        //call this during game initialization

        ////resource path
        //var targetPath = RESOURCES_DIRECTORY_PATH + ResourcePath;

        ////check to see if directory even exists
        //if (!Directory.Exists(targetPath))
        //{
        //    Debug.LogError("Unable to resolve path for " + LoggingName + " database: " + targetPath);
        //    return false;
        //}
        //else
        //{
        //    Debug.Log("Loading " + LoggingName + " database: " + targetPath);
        //}

        //load all assets of specified type and make a list
        _database = new List<DBType>(Resources.LoadAll<DBType>(ResourcePath));

        //declare database count
        Debug.Log(string.Format("Just loaded all {0} entries in {1} database.", _database.Count, LoggingName));

        return true;
    }
    public List<DBType> GetAllValues()
    {
        return new List<DBType>(_database.ToList());
    }
    public Dictionary<DBType, TValue> GetSetDictionary<TValue>(TValue initialValue = default)
    {
        var newDict = new Dictionary<DBType, TValue>();
        foreach(var item in _database)
        {
            newDict.Add(item, initialValue);
        }
        return newDict;
    }
    public bool TryFind(string internalName, out DBType foundAsset, bool caseSensitiveName = false)
    {
        if (caseSensitiveName)
        {
            foundAsset = _database.Find(v => v.name == internalName);
        }
        else
        {
            foundAsset = _database.Find(v => string.Equals(v.name, internalName, System.StringComparison.OrdinalIgnoreCase));
        }
        return foundAsset != null;
    }
    public int TryFindMany(System.Predicate<DBType> predicate, out List<DBType> foundAssets)
    {
        if (predicate != null)
        {
            foundAssets = _database.Where(v => predicate(v)).ToList();
        }
        else
        {
            foundAssets = new List<DBType>();
        }
        return foundAssets.Count;
    }
    
    public bool TryGetRandom(out DBType randomValue)
    {
        return TryGetRandom(null, out randomValue);
    }

    public bool TryGetRandom(System.Predicate<DBType> predicate, out DBType randomValue)  //where the asset matches the predicate
    {
        if (TryGetRandom(predicate, out List<DBType> found, 1) > 0)
        {
            randomValue = found[0]; //first element in a list guaranteed to have exactly 1 item.
            return true;
        }
        else
        {
            randomValue = null;
            return false;
        }
    }
    public int TryGetRandom(System.Predicate<DBType> predicate, out List<DBType> randomValues, int count = 1)
    {
        //prepare a subset of values which match the predicate, then pass to the method below. if predicate is null, use all values
        if (predicate != null)
            return TryGetRandomFromSubset(_database.Where(v => predicate(v)).ToList(), out randomValues, count);
        return TryGetRandomFromSubset(_database, out randomValues, count);
    }

    protected int TryGetRandomFromSubset(List<DBType> subset, out List<DBType> randomValues, int count)
    {
        randomValues = new List<DBType>();

        if (subset.Count > 0)
        {
            //adjust if not enough options in subset
            count = Mathf.Min(subset.Count, count);

            //for as many times as we need values
            for (int i = 0; i < count; i++)
            {
                //pick a random value from the subset
                int roll = Random.Range(0, subset.Count); //max exclusive

                //add it to the list of selected values
                randomValues.Add(subset[roll]);

                //remove it from the list of choices
                subset.RemoveAt(roll);
            }

            return count;
        }

        //return false because there were no values to select from
        return 0;
    }
#if UNITY_EDITOR
    public override List<Object> GetAllAssetsForCode()
    {
        var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(DBType).ToString()), new string[] { "Assets/Resources/" + ResourcePath });
        if (guids.Length <= 0) return new List<Object>();

        var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
        return paths.Select(path => AssetDatabase.LoadAssetAtPath<Object>(path)).ToList();
    }
#endif
}
