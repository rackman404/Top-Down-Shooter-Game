using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType{
    Spawner, Projectile, Melee
}

public interface IWeaponController
{


    public WeaponType type { get; set; }

    
    public string internalName { get; set; }

    public void Fire(Vector3 targetPos, GameObject targetObj);

    public IWeaponController Init(CharacterEntity parentE);

    public GameObject GetGameObject();

    

}
