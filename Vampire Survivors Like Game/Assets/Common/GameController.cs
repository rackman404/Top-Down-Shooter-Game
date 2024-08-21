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

       

    // Start is called before the first frame update
    void Start()
    {
        projContainerObj = new GameObject("projectile_container");
        mobContainerObj = new GameObject("mob_container");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mobEntityCount < maxMobEntityCount){
            if (Random.Range(1, 100) < spawnChance){
               Vector2 spawnPoint = Random.insideUnitCircle.normalized * Random.Range(radiusFromPlayerToSpawn-radiusFromPlayerToSpawnRange, radiusFromPlayerToSpawn + radiusFromPlayerToSpawnRange);

                Instantiate(mobSpawnList[0], spawnPoint + new Vector2(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.y), Quaternion.Euler(0f,0f,0f), mobContainerObj.transform);

            } 

        }

    }

    /// <summary>
    /// Add initialized projectile to internal game controller script.
    /// </summary>
    /// <param name="projObj"></param>
    public void addProjectile(GameObject projObj){
        projObj.transform.parent = projContainerObj.transform;
    }
}
