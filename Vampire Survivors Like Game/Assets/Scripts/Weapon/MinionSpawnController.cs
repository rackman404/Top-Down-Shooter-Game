using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawnController : MonoBehaviour , IWeaponController
{

    public GameObject mobSpawnTypePrefab;

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
            GameObject.Instantiate(mobSpawnTypePrefab, spawnPoint + new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(0f,0f,0f), GameController.Instance.levelInstance.mobContainerObj.transform).GetComponent<MobEntity>().SetParameters(parentFactionID).SetPrefabName(mobSpawnTypePrefab.name);
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
