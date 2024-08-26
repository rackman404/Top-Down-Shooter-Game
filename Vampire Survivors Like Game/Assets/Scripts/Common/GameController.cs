using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
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

    //singleton stuff
    public static GameController Instance {get; private set; }

    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    public bool gameState {get; private set;} = true;

    public bool paused {get; private set;} = false;

    public GameObject playerPrefab;

    public LevelController levelInstance {get; private set;}

    [Header("Level Instance Params")]
    public int radiusFromPlayerToSpawn;
    public int radiusFromPlayerToSpawnRange;
    public int spawnChance; //per frame

    public GameObject[] mobSpawnList;

    public int maxMobEntityCount;



    // Start is called before the first frame update
    void Start()
    {

        levelInstance = new GameObject("level_instance").transform.AddComponent<LevelController>().InitializeLevelInstance(
        radiusFromPlayerToSpawn,
        radiusFromPlayerToSpawnRange,
        spawnChance,
        mobSpawnList,
        maxMobEntityCount);

        saveLoadController = gameObject.AddComponent<SaveLoadHandler>();
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

        //save functions
        if (Input.GetKeyDown("p") && paused == true){
            levelInstance.SaveLevelData();
        }
        if (Input.GetKeyDown("o") && paused == true){
            RestartGameState();
            levelInstance.LoadLevelData();
        }
    }

    /// <summary>
    /// Invoke for a full restart of the loaded game scene
    /// </summary>
    public void RestartGameState(){  
        Destroy(levelInstance.playerInstance.gameObject);
        Destroy(levelInstance.gameObject);

        levelInstance = new GameObject("level_instance").transform.AddComponent<LevelController>().InitializeLevelInstance(
        radiusFromPlayerToSpawn,
        radiusFromPlayerToSpawnRange,
        spawnChance,
        mobSpawnList,
        maxMobEntityCount);

        gameState = true;
    }

    void OnApplicationQuit(){
        SaveAllData();
    }

    /// <summary>
    /// Autosave game prefs file data as well as level data on exit. 
    /// </summary>
    private void SaveAllData(){
        saveLoadController.SaveGamePrefs();
        levelInstance.SaveLevelData();
    }
}
