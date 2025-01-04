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

    //direct where mob should move
    public Vector3 waypoint;
    
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

    public abstract void TakeDamage(int dmg);

    public abstract void OnDeath();

    public int GetHealth(){
        return health;
    }

    public float GetSpeed(){
        return speed;
    }

    public int GetFactionID(){
        return factionID;
    }

}
