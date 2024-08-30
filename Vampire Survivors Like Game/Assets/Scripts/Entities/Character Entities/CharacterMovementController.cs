using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 targetPos, Rigidbody2D rb){
        gameObject.transform.position += targetPos;
        //rb.MovePosition(targetPos.normalized);
    }

    public void MoveTowards(Vector3 targetPos, float speed, Rigidbody2D thisRb){
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, speed * Time.deltaTime);
        //thisRb.MovePosition((gameObject.transform.position - targetPos).normalized *  speed * Time.deltaTime);
    }

}
