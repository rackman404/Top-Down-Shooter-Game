using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;


public class MobEntity : CharacterEntity
{



    private GameObject target = null;

    private EntityAiController entityAI;

    public MobEntity SetParameters(int faction){
        factionID = faction;
        

        if (factionID == 2){
            Color redShifted = new Color(255,0,0);
            gameObject.GetComponentInChildren<SpriteRenderer>().color = redShifted;
        }

        return this;
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
        
        entityAI = gameObject.AddComponent<EntityAiController>().Init(this, weaponControllers);
        
 


    }

    // Start is called before the first frame update
    void Awake()
    {

        //Init();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.levelInstance.playerInstance.isDead == false){
            
            Attack();

            //movement
            movementController.MoveTowards(waypoint, speed, rb);
            

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

        GameObject[] mobs = GameObject.FindGameObjectsWithTag("mob");
        GameObject[] player = {GameController.Instance.levelInstance.playerInstance.gameObject};

        mobs = mobs.Concat(player).ToArray();

        //targeting
        if (mobs.Length != 0){
            GameObject leastDistObj = null;
            float leastDist = System.Int32.MaxValue;
            
            for (int i = 0; i < mobs.Length; i++){

                float dist =  Vector3.Distance(mobs[i].transform.position, transform.position);
                if (dist < leastDist && this.factionID != mobs[i].GetComponent<CharacterEntity>().GetFactionID()){
                    leastDist = dist;
                    leastDistObj = mobs[i];
                }
            }

            target = leastDistObj;
        }

        if (target == null){
            for (int i = 0; i < weaponControllers.Length; i++){
                weaponControllers[i].Fire(Vector3.zero, null);
            }
        }
        else{
            for (int i = 0; i < weaponControllers.Length; i++){
                weaponControllers[i].Fire(target.transform.position, target);
            }
        }

    }


}
