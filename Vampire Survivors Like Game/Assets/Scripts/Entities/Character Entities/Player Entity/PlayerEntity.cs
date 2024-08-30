using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public partial class PlayerEntity : CharacterEntity
{

    //public params
    /// <summary>
    /// whether or not player is dead. logic dependent on if player is dead or not should be referenced from this variable through the current player instance.
    /// </summary>
    public bool isDead {get; private set;} = false;

    /// <summary>
    /// Player Instance's score.
    /// </summary>
    public int score {get; private set;} = 0;


    // Start is called before the first frame update
    void Start()
    {
        internalName = "player";
        Init();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
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

                movementController.MoveTo(movementVector, rb);
            }
            Attack();
        }
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

            for (int i = 0; i < weaponControllers.Length; i++){
                weaponControllers[i].Fire(leastDistObj.transform.position, leastDistObj);
            }
        }
    }

    /// <summary>
    /// Score to be added when projectile kills enemy. Should be invoked by a mob's death
    /// </summary>
    public void AddScore(int addS){
        score += addS;
    }


}
