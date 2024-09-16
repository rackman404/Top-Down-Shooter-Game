using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for the save load system. Note that game pref methods should only be used for application preferences and data. Use level methods for gameplay related data.
/// </summary>
public interface ISaveLoadHandler
{
    /// <summary>
    /// Returns a LevelData object
    /// </summary>
    /// <returns></returns>
    LevelData LoadLevel();

    /// <summary>
    /// Saves LevelData as binary at a preset system location.
    /// </summary>
    /// <param name="lvlData"></param>
    void SaveLevel(LevelData lvlData);

    /// <summary>
    /// Save all game pref data.
    /// </summary>
    void SaveGamePrefs();

    /// <summary>
    /// Load specific game preference value given the key. Type of returned value (e.g string, float, int) must be specified
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T LoadGamePref<T>(string key);

}
