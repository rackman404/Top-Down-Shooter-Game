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

    protected Vector2 directionVector = Vector2.zero;

    protected string originObjTag;


    IEnumerator lifetimeCounter(){
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Should only be called by itself.
    /// </summary>
    protected override void Init(){
        GameController.Instance.AddProjectile(gameObject); 
        spriteObj = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        SpriteInit();
        range = speed * lifetime;

        StartCoroutine(lifetimeCounter());
    }

    public void SetParameters(Vector2 setDirectionVec, string originTag){
        directionVector = setDirectionVec.normalized;
        originObjTag = originTag;
        

        if (originObjTag == "Player"){
            Color redShifted = new Color(255,0,0);
            gameObject.GetComponentInChildren<SpriteRenderer>().color = redShifted;
        }
    }


    
    void OnTriggerEnter2D(Collider2D collision){
        if ((collision.gameObject.CompareTag("mob") || collision.gameObject.CompareTag("Player")) && !collision.gameObject.CompareTag(originObjTag)){
            collision.gameObject.GetComponent<CharacterEntity>().TakeDamage(damage);
            Destroy(gameObject); //destroy self
        }
    }

}
