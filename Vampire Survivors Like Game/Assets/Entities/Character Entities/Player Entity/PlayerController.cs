using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterEntity
{

    //singleton stuff
    public static PlayerController Instance {get; private set; }

    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    //public params
    public float speed;
    private Vector2 playerPos = Vector2.zero; 

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

    IEnumerator PlayerAttackCycle(){
        canAttack = false;
        yield return new WaitForSeconds(weaponObjs[0].GetComponent<EntityProjectile>().fireRate); //fire rate
        canAttack = true;
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
