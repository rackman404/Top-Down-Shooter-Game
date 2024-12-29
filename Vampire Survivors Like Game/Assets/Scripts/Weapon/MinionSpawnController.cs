using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class MinionSpawnController : MonoBehaviour , IWeaponController
{
    [SerializeField]
    public string internalName { get; set; }
    public WeaponType type { get; set; }

    public int maxSpawnedMobs;

    public List<GameObject> spawnedMobs;

    public string editorInternalName;

    public GameObject mobSpawnTypePrefab;

    public GameObject summonFXPrefab;

    public float weaponCooldown;
    
    private CharacterEntity parentEntity;
    private int parentFactionID;

    private bool canSpawn = true;

    public MobEntity mobObj{get; private set;}


    void Awake(){
        mobObj = mobSpawnTypePrefab.GetComponent<MobEntity>();

        internalName = editorInternalName;
    }

    /// <summary>
    /// Pseudo constructor. Returns this to allow for method chaining.
    /// </summary>
    public IWeaponController Init(CharacterEntity parentE){
        type = WeaponType.Spawner;

        parentEntity = parentE;
        parentFactionID = parentEntity.GetFactionID();
        return this;
    }

    public void Fire(Vector3 targetPos, GameObject targetObj){ //ignore parameters; need to refactor with a proper parent class
        if (canSpawn == true){
            StartCoroutine(SpawnCycle());
            Vector2 spawnPoint = Random.insideUnitCircle.normalized * Random.Range(10f, 15f);
            StartCoroutine(SpawnMob(spawnPoint, new Vector2(transform.position.x, transform.position.y)) );
        }
    }

    /// <summary>
    /// checks if additional mobs can be spawned. Will spawn if so.
    /// </summary>
    /// <param name="spawnPoint"></param>
    /// <param name="previousEntityPos"></param>
    /// <returns></returns>
    IEnumerator SpawnMob(Vector2 spawnPoint, Vector2 previousEntityPos){


        
        for (int i = spawnedMobs.Count - 1; i > 0; i--){
            if (spawnedMobs[i] == null){
                spawnedMobs.RemoveAt(i);
            }
        }

        if (spawnedMobs.Count < maxSpawnedMobs){
                    GameObject fx = GameObject.Instantiate(summonFXPrefab, spawnPoint + new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(0f,0f,0f));
        while (fx != null){
            yield return new WaitForEndOfFrame(); //delay spawning until fx is over
        }
            
            spawnedMobs.Add(GameObject.Instantiate(mobSpawnTypePrefab, spawnPoint + previousEntityPos, Quaternion.Euler(0f,0f,0f), GameController.Instance.levelInstance.mobContainerObj.transform).GetComponent<MobEntity>().SetParameters(parentFactionID).SetPrefabName(mobSpawnTypePrefab.name).gameObject);
        }
        
        
    }

    /// <summary>
    /// Weapons cooldown coroutine.
    /// </summary>
    IEnumerator SpawnCycle(){
        canSpawn = false;
        yield return new WaitForSeconds(weaponCooldown); //fire rate
        canSpawn = true;
    }
}
