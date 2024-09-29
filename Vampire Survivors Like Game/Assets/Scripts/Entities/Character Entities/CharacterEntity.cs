using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Abstract class for mob/player entities.
/// </summary>
public abstract class CharacterEntity : Entity
{

    [Header("character entiy params")]
    [SerializeField]
    protected int health;
    [SerializeField]
    protected float speed;
    
    protected int factionID; //where faction: 1 = player, 2 = enemy

    /// <summary>
    /// Character entity attached Weapon prefabs. To be used to instantiate new weapons on runtime start.
    /// </summary>
    public GameObject[] weaponObjs;
    
    protected IWeaponController[] weaponControllers;

    /// <summary>
    /// Character attached movement controller script. Auto initialised in the Init() function;
    /// </summary>
    protected CharacterMovementController movementController;

    protected override void Init()
    {
        movementController = gameObject.AddComponent<CharacterMovementController>();

        weaponControllers = new IWeaponController[weaponObjs.Length];
        //weapon instantiate
        for (int i = 0; i < weaponObjs.Length; i++){
            weaponControllers[i] = GameObject.Instantiate(weaponObjs[i], transform.position, Quaternion.Euler(0,0,0), transform).GetComponent<IWeaponController>().Init(this);
        }
        SpriteInit();


    }

    public abstract void TakeDamage(int dmg);

    public int GetHealth(){
        return health;
    }

    public int GetFactionID(){
        return factionID;
    }

}
