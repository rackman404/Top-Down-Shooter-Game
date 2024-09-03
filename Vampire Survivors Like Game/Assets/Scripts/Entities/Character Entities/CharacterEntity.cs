using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class CharacterEntity : Entity
{

    [Header("character entiy params")]
    [SerializeField]
    protected int health;
    [SerializeField]
    protected float speed;
    
    /// <summary>
    /// Character entity attached Weapon prefabs. To be used to instantiate new weapons on runtime start.
    /// </summary>
    public GameObject[] weaponObjs;
    protected WeaponController[] weaponControllers;

    /// <summary>
    /// Character attached movement controller script. Auto initialised in the Init() function;
    /// </summary>
    protected CharacterMovementController movementController;

    protected override void Init()
    {
        movementController = gameObject.transform.gameObject.AddComponent<CharacterMovementController>();

        weaponControllers = new WeaponController[weaponObjs.Length];
        //weapon instantiate
        for (int i = 0; i < weaponObjs.Length; i++){
            weaponControllers[i] = GameObject.Instantiate(weaponObjs[i], transform.position, Quaternion.Euler(0,0,0), transform).GetComponent<WeaponController>().Init(this);
        }
        SpriteInit();
    }

    public abstract void TakeDamage(int dmg);

    public int GetHealth(){
        return health;
    }


}
