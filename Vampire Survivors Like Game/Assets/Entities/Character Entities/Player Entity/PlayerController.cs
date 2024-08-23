using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterEntity
{


    //public params
    public float speed;
    private Vector2 playerPos = Vector2.zero; 

    public bool isDead {get; private set;} = false;

    // Start is called before the first frame update
    void Start()
    {
        internalName = "player";
        Init();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        playerPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        if (isDead == false){
            if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")){
                Vector3 movementVector = Vector2.zero;

                if (Input.GetKey("w")){
                    movementVector += new Vector3 (0, speed * Time.deltaTime, 0);
                }
                if (Input.GetKey("a")){
                    movementVector += new Vector3 (-speed * Time.deltaTime, 0, 0);
                }
                if (Input.GetKey("s")){
                    movementVector += new Vector3 (0, -speed * Time.deltaTime, 0);
                }
                if (Input.GetKey("d")){
                    movementVector += new Vector3 (speed * Time.deltaTime, 0, 0);
                }

                movementController.MoveTo(movementVector);
            }

            Attack();
        }
    
       
        
    }

    IEnumerator PlayerAttackCycle(){
        canAttack = false;
        yield return new WaitForSeconds(weaponObjs[0].GetComponent<EntityProjectile>().fireRate); //fire rate
        canAttack = true;
    }

    public override void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0 && isDead == false){
            health = 0;
            isDead = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(spriteObj.gameObject);
        }
    }

    void Attack(){
        if (canAttack == true){
            StartCoroutine(PlayerAttackCycle());

            GameObject[] mobs = GameObject.FindGameObjectsWithTag("mob");

           
            if (mobs.Length != 0){
                GameObject leastDistObj = mobs[0];
                float leastDist = Int32.MaxValue;
                for (int i = 0; i < mobs.Length; i++){
                    float dist =  Vector3.Distance(mobs[i].transform.position, transform.position);
                    if (dist < leastDist){
                        leastDist = dist;
                        leastDistObj = mobs[i];
                    }
                }

                for (int i = 0; i < weaponObjs.Length; i++){
                    Instantiate(weaponObjs[i], transform.position, transform.rotation).GetComponent<EntityProjectile>().SetParameters(leastDistObj.transform.position - transform.position, gameObject.tag);
                }
            }

            
        }
    }


}
