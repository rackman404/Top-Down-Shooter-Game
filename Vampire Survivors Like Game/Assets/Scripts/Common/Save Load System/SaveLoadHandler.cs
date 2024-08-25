using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

public class SaveLoadHandler :MonoBehaviour, ISaveLoadHandler
{
    private string saveFilePath;

    void Awake(){
        saveFilePath = Application.persistentDataPath + "/save.data";
    }

    /// <summary>
    /// If save file exists at the preset system location, returns save file data as LevelData.
    /// </summary>
    /// <returns></returns>
    public LevelData LoadLevel(){
        if (File.Exists(saveFilePath)){
            //FileStream saveFileStream = new FileStream(saveFilePath, FileMode.Open);
            JsonSerializer serializer = new JsonSerializer();

            StreamReader sr = new StreamReader(saveFilePath);
            JsonReader reader = new JsonTextReader(sr);


            LevelData lvlData = (LevelData)serializer.Deserialize(reader, typeof(LevelData));

            sr.Close();

            return lvlData;
        }

        return null;
    }

    /// <summary>
    /// given a lvlData data class, serialize it in binary at a preset system location set by unity.
    /// </summary>
    /// <param name="lvlData"></param>
    public void SaveLevel(LevelData lvlData){
        Debug.Log("saving, save location: " + saveFilePath);
        //FileStream saveFileStream = new FileStream(saveFilePath, FileMode.Create);
        //saveFileStream.Close();


        JsonSerializer serializer = new JsonSerializer();

        StreamWriter sw = new StreamWriter(saveFilePath);
        JsonWriter writer = new JsonTextWriter(sw);
        writer.Formatting = Formatting.Indented;
        serializer.Serialize(writer, lvlData);

        sw.Close();
    }

    public T LoadGamePref<T>(string key){
        if (typeof(T) == typeof(float)){
            return (T)Convert.ChangeType(PlayerPrefs.GetFloat(key), typeof(T));
        }
        if (typeof(T) == typeof(string)){
            return (T)Convert.ChangeType(PlayerPrefs.GetString(key), typeof(T));
        }
        if (typeof(T) == typeof(int)){
            return (T)Convert.ChangeType(PlayerPrefs.GetInt(key), typeof(T));
        }

        Debug.LogWarning("Loaded wrong game preference type");
        return default(T);
    }

    public void SaveGamePrefs(GameController gameController){
        //total time played overall
        float temp = PlayerPrefs.GetFloat("TotalSecondsPlayed");
        PlayerPrefs.SetFloat("TotalSecondsPlayed", temp + Time.realtimeSinceStartup);

        //screen res
        PlayerPrefs.SetInt("SetScreenResHeight", Screen.height);
        PlayerPrefs.SetInt("SetScreenResWidth", Screen.width);
    }
}

