using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectileEntity : ProjectileEntity
{
    void Start(){
        Init();
    }


    void Update(){
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y) + directionVector, speed * Time.deltaTime);
    }
}
