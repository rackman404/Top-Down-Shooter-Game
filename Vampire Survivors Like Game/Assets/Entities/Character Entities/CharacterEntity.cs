using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterEntity : Entity
{
    [Header("character entiy params")]
    [SerializeField]
    protected int health;
    
    public GameObject[] weaponObjs;

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
