using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public PlayerEntity playerInstance {get; private set;}
    public GameObject projContainerObj;
    public GameObject mobContainerObj;
    public GameObject terrain;
    public LevelData lvlData;


    private int radiusFromPlayerToSpawn;
    private int radiusFromPlayerToSpawnRange;
    private int spawnChance; //per frame

    private GameObject[] mobSpawnList;

    private int maxMobEntityCount;

    private int mobEntityCount = 0;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="rfp"></param>
    /// <param name="rfpr"></param>
    /// <param name="sc"></param>
    /// <param name="msList"></param>
    /// <param name="maxMob"></param>
    /// <returns></returns>
    public LevelController InitializeLevelInstance(int rfp, int rfpr, int sc, GameObject[] msList, int maxMob){
        InitializePlayer();

        //TEMP TERRAIN
        terrain = GameObject.Find("Grass_Sample");
        
        StartCoroutine(FindTerrain());
        IEnumerator FindTerrain(){ //because load scene does not load on the same frame, must set a coroutine to set it levelscene as active scene 
            while (terrain == null){
                yield return new WaitForSeconds(0.01f);
                terrain = GameObject.Find("Grass_Sample");
            }
        }
            
        //TEMP TERRAIN
        terrain = GameObject.Find("Grass_Sample");

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

    private void InitializePlayer(){
        GameObject tempObj = Instantiate(Resources.Load("Prefabs/Entities/player") as GameObject, Vector3.zero, Quaternion.Euler(0,0,0), transform);
        playerInstance = tempObj.GetComponent<PlayerEntity>();
    }

    private void FixedUpdate()
    {
        mobEntityCount = mobContainerObj.transform.childCount;

        if (playerInstance.isDead == false){
            if (mobEntityCount < maxMobEntityCount){
                if (Random.Range(1, 100) < spawnChance){
                Vector2 spawnPoint = Random.insideUnitCircle.normalized * Random.Range(radiusFromPlayerToSpawn-radiusFromPlayerToSpawnRange, radiusFromPlayerToSpawn + radiusFromPlayerToSpawnRange);
                    if (terrain.GetComponent<SpriteRenderer>().bounds.Contains(spawnPoint)){
                        int spawnType = Random.Range(0, mobSpawnList.Length - 1);
                        Instantiate(mobSpawnList[spawnType], spawnPoint + new Vector2(playerInstance.transform.position.x, playerInstance.transform.position.y), Quaternion.Euler(0f,0f,0f), mobContainerObj.transform).GetComponent<MobEntity>().SetPrefabName(mobSpawnList[spawnType].name);
                    }  
                } 
            }
        }
    }

    /// <summary>
    /// Add initialized projectile to internal level instance container.
    /// </summary>
    /// <param name="projObj"></param>
    public void AddProjectile(GameObject projObj){
        projObj.transform.parent = projContainerObj.transform;
    }

    void Update(){
        mobEntityCount = mobContainerObj.transform.childCount;
    }

    /// <summary>
    /// Creates new empty LevelData object, 
    /// LevelData object will store entity data before having its data be serialized into a JSON file at the default persistent data path.
    /// </summary>
    public void SaveLevelData(){
        lvlData = new LevelData();
        lvlData.StoreAllEntityData(this);
        GameController.Instance.saveLoadController.SaveLevel(lvlData);
    }

    /// <summary>
    /// Creates new LevelData object from stored JSON file, 
    /// instanstiates level based off the stored LevelData object's data.
    /// </summary>
    public void LoadLevelData(){
        lvlData = GameController.Instance.saveLoadController.LoadLevel();
        if (lvlData != null){
            InstantiateLevelData();
        }
    }

    /// <summary>
    /// Instatiates all level data saved within save file.
    /// 1. instantiates mob data
    /// 2. instantiates projectile data
    /// 3. instanstiates the player
    /// </summary>
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
            //Debug.Log("Loading stored entity " + i);
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
        playerInstance.AddScore(int.Parse(lvlData.playerEntity["score"]));

        mobEntityCount = mobContainerObj.transform.childCount;
    }



}
