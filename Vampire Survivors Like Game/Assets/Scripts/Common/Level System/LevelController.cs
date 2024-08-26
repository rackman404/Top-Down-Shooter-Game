using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public PlayerEntity playerInstance {get; private set;}
    public GameObject projContainerObj;
    public GameObject mobContainerObj;
    public LevelData lvlData;


    private int radiusFromPlayerToSpawn;
    private int radiusFromPlayerToSpawnRange;
    private int spawnChance; //per frame

    private GameObject[] mobSpawnList;

    private int maxMobEntityCount;

    private int mobEntityCount = 0;

    public LevelController InitializeLevelInstance(int rfp, int rfpr, int sc, GameObject[] msList, int maxMob){
        InitializePlayer();

        

        projContainerObj = new GameObject("projectile_container");
        projContainerObj.transform.parent = transform;
        mobContainerObj = new GameObject("mob_container");
        mobContainerObj.transform.parent = transform;

        radiusFromPlayerToSpawn = rfp;
        radiusFromPlayerToSpawnRange = rfpr;
        spawnChance = sc;
        mobSpawnList = msList;
        maxMobEntityCount = maxMob;

        return this;
    }

    void InitializePlayer(){
        Debug.Log("InitializePlayer TESTSETDF");
        GameObject tempObj = Instantiate(Resources.Load("Prefabs/Entities/player") as GameObject, Vector3.zero, Quaternion.Euler(0,0,0));
        playerInstance = tempObj.GetComponent<PlayerEntity>();
    }

    void Awake()
    {
        playerInstance = GameObject.FindObjectOfType<PlayerEntity>();
    }

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
    }

    /// <summary>
    /// Add initialized projectile to internal game controller script.
    /// </summary>
    /// <param name="projObj"></param>
    public void AddProjectile(GameObject projObj){
        projObj.transform.parent = projContainerObj.transform;
    }

    void Update(){
        mobEntityCount = mobContainerObj.transform.childCount;
    }


    public void SaveLevelData(){
        lvlData = new LevelData();
        lvlData.StoreAllEntityData(this);
        GameController.Instance.saveLoadController.SaveLevel(lvlData);
    }

    public void LoadLevelData(){
        lvlData = GameController.Instance.saveLoadController.LoadLevel();
        if (lvlData != null){
            InstantiateLevelData();
        }
    }

    public void InstantiateLevelData(){
        Debug.Log("Loading level data");

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

        //mobEntityCount = mobContainerObj.transform.childCount;
    }



}
