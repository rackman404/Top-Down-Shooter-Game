using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MobEntityController : CharacterEntity
{
    [Header("mob entity params")]
    [SerializeField]
    private float speed; //m/s

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.playerInstance.isDead == false){
            Attack();
            //movement
            movementController.MoveTowards(GameController.Instance.playerInstance.transform.position, speed);
        }

    }

    public override void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0){
            Destroy(gameObject);
        }
    }


    private void Attack(){
        if (canAttack == true){
            StartCoroutine(MobAttackCycle());

            Instantiate(weaponObjs[0], transform.position, transform.rotation).GetComponent<EntityProjectile>().SetParameters(GameController.Instance.playerInstance.transform.position - transform.position, gameObject.tag);
        }
    }

    IEnumerator MobAttackCycle(){
        canAttack = false;
        yield return new WaitForSeconds(weaponObjs[0].GetComponent<EntityProjectile>().fireRate); //fire rate
        canAttack = true;
    }

}
