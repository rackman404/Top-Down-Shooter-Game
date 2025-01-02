using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : Entity
{

    [SerializeField]
    private int itemLifeTime;

    private int lifeTimeTimer;

    protected override void Init()
    {
        itemLifeTime *= LevelController.TICKSPERSECOND;
    }

    void FixedUpdate(){
        lifeTimeTimer += 1;
        if (lifeTimeTimer == itemLifeTime){
            Destroy(gameObject);
        }        
    }

    
}
