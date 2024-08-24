using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EntityProjectile : Entity
{
    [Header("projectile entity params")]
    public float speed;

    public int damage;

    public float lifetime;

    public float fireRate;

    public int projectilesFiredPerAttack;

    public float range {get; private set; }

    private Vector2 directionVector = Vector2.zero;

    private string originObjTag;

    void Start(){
        Init();
    }

    IEnumerator lifetimeCounter(){
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    protected override void Init(){
        GameController.Instance.AddProjectile(gameObject);
        range = speed * lifetime;
        spriteObj = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        SpriteInit();

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

    void Update(){
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y) + directionVector, speed * Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D collision){
        if ((collision.gameObject.CompareTag("mob") || collision.gameObject.CompareTag("Player")) && !collision.gameObject.CompareTag(originObjTag)){
            collision.gameObject.GetComponent<CharacterEntity>().TakeDamage(damage);
            Destroy(gameObject); //destroy self
        }
    }

}
