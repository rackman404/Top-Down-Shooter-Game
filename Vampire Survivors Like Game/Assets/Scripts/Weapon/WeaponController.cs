using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WeaponController : MonoBehaviour, IWeaponController
{

    //public params
    public GameObject projectilePrefab;

    public float weaponCooldown;

    private WeaponSoundController soundController;
    
    private CharacterEntity parentEntity;
    private bool canAttack = true;

    public ProjectileEntity projObj{get; private set;}

    private int parentFactionID;

    void Awake(){
        projObj = projectilePrefab.GetComponent<ProjectileEntity>();
        soundController = gameObject.AddComponent<WeaponSoundController>();
    }

    /// <summary>
    /// Pseudo constructor. Returns this to allow for method chaining.
    /// </summary>
    public IWeaponController Init(CharacterEntity parentE){
        parentEntity = parentE;
        parentFactionID = parentEntity.GetFactionID();

        return this;
    }

    void FixedUpdate(){
        if (parentFactionID == 0){
            parentFactionID = parentEntity.GetFactionID();
        }
    }

    /// <summary>
    /// Fire projectiles at target vector position given. Will not fire if on cooldown or projectiles are out of range.
    /// </summary>
    /// <param name="targetPos"></param>
    public void Fire(Vector3 targetPos, GameObject targetObj){
        if (canAttack == true){
            if (projectilePrefab.GetComponent<ProjectileEntity>().speed * projectilePrefab.GetComponent<ProjectileEntity>().lifetime >= Vector3.Distance(targetPos, parentEntity.transform.position)){
                StartCoroutine(AttackCycle());

                soundController.FireTriggerSFX();
                Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<ProjectileEntity>().SetParameters(targetPos - parentEntity.transform.position, parentEntity.tag, targetObj, projectilePrefab.name, parentFactionID);
            }
        }
    }

    /// <summary>
    /// Weapons cooldown coroutine.
    /// </summary>
    IEnumerator AttackCycle(){
        canAttack = false;
        yield return new WaitForSeconds(weaponCooldown); //fire rate
        canAttack = true;
    }


}
