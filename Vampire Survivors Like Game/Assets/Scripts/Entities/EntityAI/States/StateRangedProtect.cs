using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common;
using UnityEngine;

/// <summary>
/// Protects a given reference entity (circles around it)
/// </summary>
public class StateRangedProtect: MonoBehaviour, IAIState
{

    private const int UPPERBOUND = 60;
    private const int LOWERBOUND = 25;

    private static System.Random rng = new System.Random();
    private bool pickNewRadius = true;
    private float orbitRadius;
    private float orbitDirection;

    Entity target = GameController.Instance.levelInstance.playerInstance;


    public void OnBehaviour(Entity refEntity){
        MobEntity e = refEntity as MobEntity;

        //move closer
        if (Vector3.Distance(refEntity.transform.position, target.transform.position) > UPPERBOUND ){

            e.waypoint = target.transform.position;

            pickNewRadius = true;
        }

        //move farther
        else if (Vector3.Distance(refEntity.transform.position, target.transform.position) < LOWERBOUND){
            
            e.waypoint = e.transform.position + (e.transform.position - target.transform.position);
            
            pickNewRadius = true;
        }

        //move circularly
        else {
            CharacterEntity g = refEntity as MobEntity;
            float currentAngle =  Mathf.Atan2(refEntity.transform.position.x - target.transform.position.x, refEntity.transform.position.y - target.transform.position.y);
            
            if (pickNewRadius == true){
                orbitRadius = rng.Next(LOWERBOUND, UPPERBOUND);
                
                int temp = rng.Next(0,2);
                if (temp == 0){
                    orbitDirection = -1;
                }
                else{   
                    orbitDirection = 1;
                }
                pickNewRadius = false;
            }

            if (Vector3.Distance(refEntity.transform.position, target.transform.position) < orbitRadius){ // if distance is lower than intended orbit distance
                e.waypoint = e.transform.position + (e.transform.position - target.transform.position);
            }
            else{
                e.waypoint = new Vector3(
                orbitRadius * Mathf.Sin(currentAngle + orbitDirection * g.GetSpeed() * 100) + target.transform.position.x,
                orbitRadius * Mathf.Cos(currentAngle + orbitDirection * g.GetSpeed() * 100) + target.transform.position.y);
            }


        }
    }


}
