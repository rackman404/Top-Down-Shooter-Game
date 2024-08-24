using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameController : MonoBehaviour
{



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

    private GameObject projContainerObj;
    private GameObject mobContainerObj;

    public PlayerController playerInstance {get; private set;}

    public bool gameState = true;


    public GameObject playerPrefab;



    // Start is called before the first frame update
    void Start()
    {
        projContainerObj = new GameObject("projectile_container");
        projContainerObj.transform.parent = transform;
        mobContainerObj = new GameObject("mob_container");
        mobContainerObj.transform.parent = transform;

        playerInstance = GameObject.FindObjectOfType<PlayerController>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerInstance.isDead == false){


            if (mobEntityCount < maxMobEntityCount){
                if (Random.Range(1, 100) < spawnChance){
                Vector2 spawnPoint = Random.insideUnitCircle.normalized * Random.Range(radiusFromPlayerToSpawn-radiusFromPlayerToSpawnRange, radiusFromPlayerToSpawn + radiusFromPlayerToSpawnRange);
                    Instantiate(mobSpawnList[0], spawnPoint + new Vector2(playerInstance.transform.position.x, playerInstance.transform.position.y), Quaternion.Euler(0f,0f,0f), mobContainerObj.transform);
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

        playerInstance = tempObj.GetComponent<PlayerController>();

        gameState = true;
    }
}
