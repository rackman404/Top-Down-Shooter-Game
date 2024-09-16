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

        saveLoadController = gameObject.AddComponent<SaveLoadHandler>();
    }

    void Start(){
        if (Application.isEditor == false){
            if (SceneManager.GetSceneByName("GUIScene").isLoaded == false){
                SceneManager.LoadScene("GUIScene", LoadSceneMode.Additive);

                paused = true;

                /*
                SceneManager.LoadScene("LevelScene", LoadSceneMode.Additive);

                StartCoroutine(SetActive());
            
                IEnumerator SetActive(){ //because load scene does not load on the same frame, must set a coroutine to set it levelscene as active scene 
                    bool done = false;
                    while (done == false){
                        yield return new WaitForSeconds(0.01f);
                        if (SceneManager.GetSceneByName("LevelScene").isLoaded == true){
                            Scene levelScene = SceneManager.GetSceneByName("LevelScene");
                            SceneManager.SetActiveScene(levelScene);
                            done = true;

                            levelInstance = new GameObject("level_instance").transform.gameObject.AddComponent<LevelController>().InitializeLevelInstance(
                            radiusFromPlayerToSpawn,
                            radiusFromPlayerToSpawnRange,
                            spawnChance,
                            mobSpawnList,
                            maxMobEntityCount);
                        }
                    }
                }
                */
            }
        }
        else{
            Scene levelScene = SceneManager.GetSceneByName("LevelScene");
            SceneManager.SetActiveScene(levelScene);

            levelInstance = new GameObject("level_instance").transform.gameObject.AddComponent<LevelController>().InitializeLevelInstance(
            radiusFromPlayerToSpawn,
            radiusFromPlayerToSpawnRange,
            spawnChance,
            mobSpawnList,
            maxMobEntityCount);
        }

        
    }

    void OnApplicationQuit(){
        if (levelInstance != null){
            SaveAllData();
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (levelInstance != null){
            if (levelInstance.playerInstance.isDead == true){
                gameState = false;

                if (Input.GetKey("space")){
                    RestartGameState();
                }
            }    
        }
    }

    void Update(){
        if (levelInstance != null){
            if (Input.GetKeyDown("escape")){
                if (paused == false){
                    Pause();
                }
                else{
                    Resume();
                } 
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

        if (SceneManager.GetSceneByName("LevelScene").isLoaded == false){
            SceneManager.LoadScene("LevelScene", LoadSceneMode.Additive);
            
            StartCoroutine(SetActive());
            
            IEnumerator SetActive(){ //because load scene does not load on the same frame, must set a coroutine to set it levelscene as active scene 
                bool done = false;
                while (done == false){
                    yield return new WaitForSeconds(0.01f);
                    if (SceneManager.GetSceneByName("LevelScene").isLoaded == true){
                        Scene levelScene = SceneManager.GetSceneByName("LevelScene");
                        SceneManager.SetActiveScene(levelScene);
                        done = true;

                        levelInstance = new GameObject("level_instance").transform.gameObject.AddComponent<LevelController>().InitializeLevelInstance(
                        radiusFromPlayerToSpawn,
                        radiusFromPlayerToSpawnRange,
                        spawnChance,
                        mobSpawnList,
                        maxMobEntityCount);
                    }
                }
            }
        }
        else {
            levelInstance = new GameObject("level_instance").transform.gameObject.AddComponent<LevelController>().InitializeLevelInstance(
            radiusFromPlayerToSpawn,
            radiusFromPlayerToSpawnRange,
            spawnChance,
            mobSpawnList,
            maxMobEntityCount);

            //unpause and set game as valid
        }

        //unpause and set game as valid
        Resume();
        gameState = true;

        
    }

    /// <summary>
    /// Autosave game prefs file data as well as level data on exit. 
    /// </summary>
    private void SaveAllData(){
        saveLoadController.SaveGamePrefs();
        levelInstance.SaveLevelData();
    }


    public void ExitLevel(){
        Camera.main.transform.parent = this.transform;
        SceneManager.UnloadSceneAsync("LevelScene");
    }

    public void Resume(){
        Time.timeScale = 1;
        paused = false;
    }

    public void Pause(){
        Time.timeScale = 0;
        paused = true;
    }
}
