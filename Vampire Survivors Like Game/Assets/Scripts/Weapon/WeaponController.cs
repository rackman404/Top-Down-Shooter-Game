using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    //public params
    public GameObject projectilePrefabs;
    public float weaponCooldown;

    private Entity parentEntity;

    private bool canAttack = true;

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
    public void Fire(Vector3 targetPos){
         if (canAttack == true){
            if (projectilePrefabs.GetComponent<ProjectileEntity>().speed * projectilePrefabs.GetComponent<ProjectileEntity>().lifetime >= Vector3.Distance(targetPos, parentEntity.transform.position)){
                StartCoroutine(AttackCycle());
                Instantiate(projectilePrefabs, transform.position, transform.rotation).GetComponent<ProjectileEntity>().SetParameters(targetPos - parentEntity.transform.position, parentEntity.tag);
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
