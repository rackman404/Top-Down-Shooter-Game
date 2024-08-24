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

    public void MoveTo(Vector3 targetPos){
        gameObject.transform.position += targetPos;
    }

    public void MoveTowards(Vector3 targetPos, float speed){
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, speed * Time.deltaTime);
    }

}
