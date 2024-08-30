using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MobEntity : CharacterEntity
{

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }



    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.levelInstance.playerInstance.isDead == false){
            Attack();
            //movement
            movementController.MoveTowards(GameController.Instance.levelInstance.playerInstance.transform.position, speed, rb);
        }
    }

    /// <summary>
    /// Subtract damage taken to entity's health.
    /// </summary>
    /// <param name="dmg"></param>
    public override void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0){
            GameController.Instance.levelInstance.playerInstance.AddScore(5);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Will invoke attached WeaponControllers and their firing logic methods as needed.
    /// </summary>
    private void Attack(){
        for (int i = 0; i < weaponControllers.Length; i++){
            weaponControllers[i].Fire(GameController.Instance.levelInstance.playerInstance.transform.position, GameController.Instance.levelInstance.playerInstance.gameObject);
        }
    }


}
