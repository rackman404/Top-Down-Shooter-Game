using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{

    public void MoveTo(Vector3 targetPos, Rigidbody2D rb){
        //gameObject.transform.position += targetPos;
        rb.velocity = targetPos;
        //rb.AddForce(targetPos.normalized);
        //rb.AddForce(transform.up * 1);
    }

    public void MoveTowards(Vector3 targetPos, float speed, Rigidbody2D thisRb){
        //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, speed * Time.deltaTime);
        thisRb.velocity = (targetPos - thisRb.gameObject.transform.position).normalized * speed;
    }

}
