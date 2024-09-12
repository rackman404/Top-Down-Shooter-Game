using System.Collections;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WeaponController : MonoBehaviour
{

    //public params
    public GameObject projectilePrefab;

    public float weaponCooldown;
    

    private Entity parentEntity;
    private bool canAttack = true;

    public ProjectileEntity projObj{get; private set;}


    void Awake(){
        projObj = projectilePrefab.GetComponent<ProjectileEntity>();
    }

    /// <summary>
    /// Pseudo constructor. Returns this to allow for method chaining.
    /// </summary>
    public WeaponController Init(Entity parentE){
        parentEntity = parentE;
        return this;
    }

    /// <summary>
    /// Fire projectiles at target vector position given. Will not fire if on cooldown or projectiles are out of range.
    /// </summary>
    /// <param name="targetPos"></param>
    public void Fire(Vector3 targetPos, GameObject targetObj){
         if (canAttack == true){
            if (projectilePrefab.GetComponent<ProjectileEntity>().speed * projectilePrefab.GetComponent<ProjectileEntity>().lifetime >= Vector3.Distance(targetPos, parentEntity.transform.position)){
                StartCoroutine(AttackCycle());
                Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<ProjectileEntity>().SetParameters(targetPos - parentEntity.transform.position, parentEntity.tag, targetObj, projectilePrefab.name);
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
