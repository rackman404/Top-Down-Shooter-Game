using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterEntity : Entity
{
    [Header("character entiy params")]
    [SerializeField]
    private int health;
    
    public GameObject[] weaponObjs;

    protected CharacterMovementController movementController;

    protected bool canAttack = true;

    protected override void Init()
    {
        movementController = gameObject.transform.AddComponent<CharacterMovementController>();
        
        SpriteInit();
    }

    public void TakeDamage(int dmg){
        health -= dmg;

        if (health <= 0){
            Destroy(gameObject);
        }
    }

    public int GetHealth(){
        return health;
    }


}
