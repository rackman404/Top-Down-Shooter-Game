using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameController : MonoBehaviour
{

    private ISaveLoadHandler saveLoadController;

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

    public int radiusFromPlayerToSpawn;
    public int radiusFromPlayerToSpawnRange;

    public int spawnChance; //per frame

    public GameObject[] mobSpawnList;

    public int maxMobEntityCount;

    private int mobEntityCount = 0;

    public GameObject projContainerObj;
    public GameObject mobContainerObj;

    public PlayerEntity playerInstance {get; private set;}

    public bool gameState {get; private set;} = true;

    public bool paused {get; private set;} = false;

    public GameObject playerPrefab;


    // Start is called before the first frame update
    void Start()
    {
        projContainerObj = new GameObject("projectile_container");
        projContainerObj.transform.parent = transform;
        mobContainerObj = new GameObject("mob_container");
        mobContainerObj.transform.parent = transform;

        playerInstance = GameObject.FindObjectOfType<PlayerEntity>();
        saveLoadController = gameObject.AddComponent<SaveLoadHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mobEntityCount = mobContainerObj.transform.childCount;

        if (playerInstance.isDead == false){
            if (mobEntityCount < maxMobEntityCount){
                if (Random.Range(1, 100) < spawnChance){
                Vector2 spawnPoint = Random.insideUnitCircle.normalized * Random.Range(radiusFromPlayerToSpawn-radiusFromPlayerToSpawnRange, radiusFromPlayerToSpawn + radiusFromPlayerToSpawnRange);
                    
                    int spawnType = Random.Range(0, mobSpawnList.Length - 1);
                    Instantiate(mobSpawnList[spawnType], spawnPoint + new Vector2(playerInstance.transform.position.x, playerInstance.transform.position.y), Quaternion.Euler(0f,0f,0f), mobContainerObj.transform).GetComponent<MobEntity>().SetPrefabName(mobSpawnList[spawnType].name);
                } 
            }
        }
        else{
            gameState = false;

            if (Input.GetKey("space")){
                RestartGameState();
            }
        }
    }

    void Update(){
        mobEntityCount = mobContainerObj.transform.childCount;

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
            SaveLevelData();
        }
        if (Input.GetKeyDown("o") && paused == true){
            LoadLevelData();
        }
    }

    /// <summary>
    /// Add initialized projectile to internal game controller script.
    /// </summary>
    /// <param name="projObj"></param>
    public void AddProjectile(GameObject projObj){
        projObj.transform.parent = projContainerObj.transform;
    }

    /// <summary>
    /// Invoke for a full restart of the loaded game scene
    /// </summary>
    public void RestartGameState(){
        
        for (int i = 0; i < mobContainerObj.transform.childCount; i++){
            Destroy(mobContainerObj.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < projContainerObj.transform.childCount; i++){
            Destroy(projContainerObj.transform.GetChild(i).gameObject);
        }

        Destroy(playerInstance.gameObject);

        GameObject tempObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.Euler(0,0,0));

        playerInstance = tempObj.GetComponent<PlayerEntity>();

        gameState = true;
    }


    void OnApplicationQuit(){
        SaveAllData();
    }


    /// <summary>
    /// Autosave game prefs file data as well as level data on exit. 
    /// </summary>
    private void SaveAllData(){
        saveLoadController.SaveGamePrefs(this);
        lvlData.StoreAllEntityData(this);
    }

    private void SaveLevelData(){
        lvlData = new LevelData();
        lvlData.StoreAllEntityData(this);

        saveLoadController.SaveLevel(lvlData);
    }

    private void LoadLevelData(){
        RestartGameState();

        lvlData = saveLoadController.LoadLevel();
        if (lvlData != null){
            InstantiateLevelData();
        }
        
    }

    private void InstantiateLevelData(){
        Debug.Log("Loading level data");
        Debug.Log(lvlData.mobEntities[0]["prefab name"]);

        //initialize mob data
        for (int i = 0; i < lvlData.mobEntities.Length; i++){
            Debug.Log("Loading stored entity " + i);
            GameObject temp = Instantiate(Resources.Load("Prefabs/Entities/Mob/" + lvlData.mobEntities[i]["prefab name"]) as GameObject, mobContainerObj.transform);
            temp.transform.position = new Vector3(float.Parse(lvlData.mobEntities[i]["transform x"]), float.Parse(lvlData.mobEntities[i]["transform y"]), 0);
            temp.GetComponent<MobEntity>().TakeDamage(temp.GetComponent<MobEntity>().GetHealth() - int.Parse(lvlData.mobEntities[i]["health"]));
            temp.GetComponent<MobEntity>().SetPrefabName(lvlData.mobEntities[i]["prefab name"]);
        }

        //initialize projectile data
        for (int i = 0; i < lvlData.projectileEntities.Length; i++){
            Debug.Log("Loading stored entity " + i);
            GameObject temp = Instantiate(Resources.Load("Prefabs/Entities/Projectile/" + lvlData.projectileEntities[i]["prefab name"]) as GameObject, projContainerObj.transform);

            temp.GetComponent<ProjectileEntity>().SetParameters(
                new Vector3 (float.Parse(lvlData.projectileEntities[i]["direction vec x"]), float.Parse(lvlData.projectileEntities[i]["direction vec y"]), 0),
                lvlData.projectileEntities[i]["origin tag"],
                null,
                lvlData.projectileEntities[i]["prefab name"]
                );

            temp.transform.position = new Vector3(float.Parse(lvlData.projectileEntities[i]["transform x"]), float.Parse(lvlData.projectileEntities[i]["transform y"]), 0);
        }

        //initialize player data
        playerInstance.transform.position = new Vector3(float.Parse(lvlData.playerEntity["transform x"]), float.Parse(lvlData.playerEntity["transform y"]), 0);
        playerInstance.TakeDamage(playerInstance.GetHealth() - int.Parse(lvlData.playerEntity["health"]));

        mobEntityCount = mobContainerObj.transform.childCount;
    }

}
