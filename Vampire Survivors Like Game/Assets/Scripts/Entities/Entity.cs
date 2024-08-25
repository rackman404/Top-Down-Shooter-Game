using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    protected SpriteRenderer spriteObj;
    protected BoxCollider2D entityCollider;

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
        PolygonCollider2D temp = spriteObj.transform.AddComponent<PolygonCollider2D>();
        entityCollider = transform.AddComponent<BoxCollider2D>();
        entityCollider.size = temp.bounds.size;
        entityCollider.isTrigger = true;
        Destroy(temp);

        rb = gameObject.transform.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    public string GetInternalName(){
        return internalName;
    }

    public Entity SetPrefabName(string str){
        prefabName = str;
        return this;
    }
    
}
