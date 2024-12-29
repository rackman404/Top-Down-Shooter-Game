using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateType{
    RangedProtect, Ranged, MeleeProtect, Melee
}


public class EntityAiController : MonoBehaviour
{

    
    private MobEntity attachedEntity;

    private IWeaponController[] attachedWeapons;

    private bool ally = false;

    private IAIState currentState; 

    

    public EntityAiController Init(MobEntity e, IWeaponController[] w){
        attachedEntity = e;
        attachedWeapons = w;


        if (e.GetFactionID() == 1){
            ally = true;
        }

        InitialState();

        return this;
    }




    private void InitialState(){

        //only first weapon is checked for ai state determination

        switch(attachedWeapons[0].type){
            case WeaponType.Projectile:
                if (ally == true){
                    currentState = gameObject.AddComponent<StateRangedProtect>();
                }
                else{
                    currentState = gameObject.AddComponent<StateRanged>();
                }
                break;
            default:
                Debug.LogWarning("weapon type not defined");
                break;

        }
    }

    public void SwitchState(StateType type){

    }


    // Update is called once per frame
    void Update()
    {
        currentState.OnBehaviour(attachedEntity);
    }
}
