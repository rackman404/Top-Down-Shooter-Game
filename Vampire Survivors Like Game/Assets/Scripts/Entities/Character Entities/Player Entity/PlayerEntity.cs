using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;



public partial class PlayerEntity : CharacterEntity
{

    #region DEBUG STUFF

    private Vector3 previousInput = Vector3.zero; 

    public void SetPreviousInput(string str){
        previousInput = Vector3.zero; 
    
        if (str.Contains("r") && isDead == true){
            GameController.Instance.RestartGameState();     
        }

        if (str.Contains("w")){
        previousInput += new Vector3 (0, speed, 0);
        }
        if (str.Contains("a")){
            previousInput += new Vector3 (-speed, 0, 0);
        }
        if (str.Contains("s")){
            previousInput += new Vector3 (0, -speed, 0);
        }
        if (str.Contains("d")){
            previousInput += new Vector3 (speed, 0, 0);
        }
      
        

    }

    #endregion

    //public params
    /// <summary>
    /// whether or not player is dead. logic dependent on if player is dead or not should be referenced from this variable through the current player instance.
    /// </summary>
    public bool isDead {get; private set;} = false;

    public int score {get; private set;} = 0;

    public int totalKills {get; private set;} = 0;

    public float timeAlive {get; private set;} = 0;

    void Awake()
    {
        internalName = "player";
        factionID = 1;

    }

    void Update(){
        timeAlive += Time.deltaTime;
        
    }

    protected override void Init()
    {
        waypoint = transform.position; //set initial position point at current pos
        movementController = gameObject.AddComponent<CharacterMovementController>();

        weaponControllers = new IWeaponController[weaponObjs.Length];
        //weapon instantiate
        for (int i = 0; i < weaponObjs.Length; i++){
            weaponControllers[i] = GameObject.Instantiate(weaponObjs[i], transform.position, Quaternion.Euler(0,0,0), transform).GetComponent<IWeaponController>().Init(this);
        }
        
       


    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        if (isDead == false){
            if (GameController.Instance.DEBUGMODE == false){

                    if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")){
                    Vector3 movementVector = Vector2.zero;

                    if (Input.GetKey("w")){
                        movementVector += new Vector3 (0, speed, 0);
                    }
                    if (Input.GetKey("a")){
                        movementVector += new Vector3 (-speed, 0, 0);
                    }
                    if (Input.GetKey("s")){
                        movementVector += new Vector3 (0, -speed, 0);
                    }
                    if (Input.GetKey("d")){
                        movementVector += new Vector3 (speed, 0, 0);
                    }

                    movementController.MoveTo(movementVector, rb);
                }
                else{
                    movementController.MoveTo(new Vector2(), rb);
                }
            }
            else{
                movementController.MoveTo(previousInput, rb);       
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

            rb.velocity = Vector2.zero;

            SoundManager.Instance.BeginGameOverSFX();

            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(spriteObj.gameObject);
        }
    }

    void Attack(){
        GameObject[] mobs = GameObject.FindGameObjectsWithTag("mob");



        if (mobs.Length != 0){
            GameObject leastDistObj = null;
            float leastDist = Int32.MaxValue;
            
            for (int i = 0; i < mobs.Length; i++){
                float dist =  Vector3.Distance(mobs[i].transform.position, transform.position);
                if (dist < leastDist && this.factionID != mobs[i].GetComponent<CharacterEntity>().GetFactionID()){
                    leastDist = dist;
                    leastDistObj = mobs[i];
                }
            }

            if (leastDistObj == null){
                for (int i = 0; i < weaponControllers.Length; i++){
                    weaponControllers[i].Fire(Vector3.zero, null);
                }   
            }
            else{
                for (int i = 0; i < weaponControllers.Length; i++){
                    weaponControllers[i].Fire(leastDistObj.transform.position, leastDistObj);
                }
            }

        }
        else{ //default if no mobs on map
            for (int i = 0; i < weaponControllers.Length; i++){
                weaponControllers[i].Fire(Vector3.zero, null);
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
