using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class MinionSpawnController : MonoBehaviour , IWeaponController
{

    public GameObject mobSpawnTypePrefab;

    public GameObject summonFXPrefab;

    public float weaponCooldown;
    
    private CharacterEntity parentEntity;
    private int parentFactionID;

    private bool canSpawn = true;

    public MobEntity mobObj{get; private set;}


    void Awake(){
        mobObj = mobSpawnTypePrefab.GetComponent<MobEntity>();
    }

    /// <summary>
    /// Pseudo constructor. Returns this to allow for method chaining.
    /// </summary>
    public IWeaponController Init(CharacterEntity parentE){
        parentEntity = parentE;
        parentFactionID = parentEntity.GetFactionID();
        return this;
    }

    public void Fire(Vector3 targetPos, GameObject targetObj){ //ignore parameters; need to refactor with a proper parent class
        if (canSpawn == true){
            StartCoroutine(SpawnCycle());
            Vector2 spawnPoint = Random.insideUnitCircle.normalized * Random.Range(10f, 15f);
            StartCoroutine(SpawnMob(spawnPoint));
        }
    }

    IEnumerator SpawnMob(Vector2 spawnPoint){
        GameObject fx = GameObject.Instantiate(summonFXPrefab, spawnPoint + new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(0f,0f,0f));
        while (fx != null){
            yield return new WaitForEndOfFrame();
        }
        GameObject.Instantiate(mobSpawnTypePrefab, spawnPoint + new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(0f,0f,0f), GameController.Instance.levelInstance.mobContainerObj.transform).GetComponent<MobEntity>().SetParameters(parentFactionID).SetPrefabName(mobSpawnTypePrefab.name);
        

        
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
