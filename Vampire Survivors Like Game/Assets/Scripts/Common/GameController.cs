using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameController : MonoBehaviour
{

    public ISaveLoadHandler saveLoadController {get; private set;}

    /// <summary>
    /// Stores level data. Should ONLY be interacted with on saving and on loading.
    /// </summary>
    private LevelData lvlData;

    public static GameController Instance {get; private set; }

    public bool gameState {get; private set;} = true;

    public bool paused {get; private set;} = false;

    [Header("Game Params")]
    public GameObject playerPrefab;
    public Scene GUIScenePrefab;

    public LevelController levelInstance {get; private set;}

    [Header("Level Instance Params")]
    public int radiusFromPlayerToSpawn;
    public int radiusFromPlayerToSpawnRange;
    public int spawnChance; //per frame

    public GameObject[] mobSpawnList;

    public int maxMobEntityCount;


    void Awake()
    {
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    
        if (Application.isEditor == false){
            if (SceneManager.GetSceneByName("GUIScene").isLoaded == false){
                SceneManager.LoadScene("GUIScene", LoadSceneMode.Additive);
            }
        }

        



        levelInstance = new GameObject("level_instance").transform.gameObject.AddComponent<LevelController>().InitializeLevelInstance(
        radiusFromPlayerToSpawn,
        radiusFromPlayerToSpawnRange,
        spawnChance,
        mobSpawnList,
        maxMobEntityCount);

        saveLoadController = gameObject.AddComponent<SaveLoadHandler>();
    }

    void OnApplicationQuit(){
        SaveAllData();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (levelInstance.playerInstance.isDead == true){
            gameState = false;

            if (Input.GetKey("space")){
                RestartGameState();
            }
        }    
    }

    void Update(){
        if (Input.GetKeyDown("escape")){
            if (paused == false){
                Time.timeScale = 0;
                paused = true;
            }
            else{
                Time.timeScale = 1;
                paused = false;
            } 
        }

        //save functions upon key press. TODO: move from key press to actual GUI interactions
        if (Input.GetKeyDown("p") && paused == true && !levelInstance.playerInstance.isDead){
            levelInstance.SaveLevelData();
        }
        if (Input.GetKeyDown("o") && paused == true){
            RestartGameState();
            levelInstance.LoadLevelData();
        }
    }

    /// <summary>
    /// Invoke for a full restart of the loaded game scene.
    /// Will create destroy existing player and level instances before reinitializing new level and player instances.
    /// </summary>
    public void RestartGameState(){  
        if (levelInstance != null){
            Destroy(levelInstance.gameObject);
        }
        

        levelInstance = new GameObject("level_instance").transform.gameObject.AddComponent<LevelController>().InitializeLevelInstance(
        radiusFromPlayerToSpawn,
        radiusFromPlayerToSpawnRange,
        spawnChance,
        mobSpawnList,
        maxMobEntityCount);

        gameState = true;
    }

    /// <summary>
    /// Autosave game prefs file data as well as level data on exit. 
    /// </summary>
    private void SaveAllData(){
        saveLoadController.SaveGamePrefs();
        levelInstance.SaveLevelData();
    }
}
