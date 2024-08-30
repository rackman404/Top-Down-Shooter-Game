using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class ProjectileEntity : Entity
{
    [Header("projectile entity params")]
    public float speed;

    public int damage;

    public float lifetime;

    public float fireRate;

    public int projectilesFiredPerAttack;

    [SerializeField]
    public float range {get; private set; }

    public Vector2 directionVector {get; protected set;}= Vector2.zero;

    public string originObjTag {get; protected set;}

    /// <summary>
    /// reference to intended target entity. If used, it should be checked to see if it is null or destroyed first.
    /// </summary>
    public GameObject originalTargetObj {get; protected set;}


    IEnumerator lifetimeCounter(){
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Should only be called by itself.
    /// </summary>
    protected override void Init(){
        

        GameController.Instance.levelInstance.AddProjectile(gameObject); 
        spriteObj = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        SpriteInit();
        GetComponent<BoxCollider2D>().isTrigger = true;

        range = speed * lifetime;

        StartCoroutine(lifetimeCounter());
    }

    /// <summary>
    /// Called when Projectile is 
    /// </summary>
    /// <param name="setDirectionVec"></param>
    /// <param name="originTag"></param>
    public void SetParameters(Vector2 setDirectionVec, string originTag, GameObject oTargetObj, string prefabNameStr){
        directionVector = setDirectionVec.normalized;
        originObjTag = originTag;
        originalTargetObj = oTargetObj;
        prefabName = prefabNameStr;        

        if (originObjTag == "Player"){
            Color redShifted = new Color(255,0,0);
            gameObject.GetComponentInChildren<SpriteRenderer>().color = redShifted;
        }
    }


    /// <summary>
    /// Base collision damage logic.
    /// Should be overridden as need by extending projectile classes as needed for custom damage logic.
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision){
        if ((collision.gameObject.CompareTag("mob") || collision.gameObject.CompareTag("Player")) && !collision.gameObject.CompareTag(originObjTag)){
            collision.gameObject.GetComponent<CharacterEntity>().TakeDamage(damage);
            Destroy(gameObject); //destroy self
        }
        /*
        else if ({

        }
        */
    }

}
