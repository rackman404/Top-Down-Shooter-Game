using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    protected SpriteRenderer spriteObj;
    protected BoxCollider2D entityCollider;

    protected Animator entityAnim;

    protected EntityAnimationController entityAnimController;

    protected Rigidbody2D rb;

    [Header("entiy params")]
    public string internalName = "";
    public string prefabName {get; protected set;}= "";

    protected abstract void Init(); //should be overridden and used as a constructor within extending entity classes

    /// <summary>
    /// Should only be called by CharacterEntity and ProjectileEntity extended classes.
    /// </summary>
    protected void SpriteInit(){

        

        spriteObj = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        PolygonCollider2D temp = spriteObj.transform.gameObject.AddComponent<PolygonCollider2D>();
        entityCollider = transform.gameObject.AddComponent<BoxCollider2D>();
        entityCollider.size = temp.bounds.size;
        entityCollider.isTrigger = false;
        Destroy(temp);

        rb = gameObject.transform.gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        
        
        
        entityAnim = gameObject.GetComponentInChildren<Animator>();

        if (entityAnim != null){
            entityAnimController = gameObject.AddComponent<EntityAnimationController>().Constructor(entityAnim, spriteObj.sprite.name.TrimEnd('_','0'), rb);
        }
    }

    public string GetInternalName(){
        return internalName;
    }

    public Entity SetPrefabName(string str){
        prefabName = str;
        return this;
    }
}
