using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterEntity : Entity
{
    [Header("character entiy params")]
    [SerializeField]
    protected int health;
    [SerializeField]
    protected float speed;
    
    /// <summary>
    /// Character entity attached projectile prefabs. To be used to instantiate new projectiles.
    /// </summary>
    public GameObject[] weaponObjs;

    /// <summary>
    /// Character attached movement controller script. Auto initialised in the Init() function;
    /// </summary>
    protected CharacterMovementController movementController;

    protected bool canAttack = true;

    protected override void Init()
    {
        movementController = gameObject.transform.AddComponent<CharacterMovementController>();
        
        SpriteInit();
    }

    public abstract void TakeDamage(int dmg);

    public int GetHealth(){
        return health;
    }


}
