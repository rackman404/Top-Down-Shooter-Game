using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRanged : MonoBehaviour, IAIState
{
    Entity target = GameController.Instance.levelInstance.playerInstance;


    public void OnBehaviour(Entity refEntity){
        MobEntity e = refEntity as MobEntity;
        e.waypoint = target.transform.position;
    }

}
