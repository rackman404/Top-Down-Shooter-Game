using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateType{
    RangedProtect, Ranged, MeleeProtect, Melee, Idle, AIDisabled
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

        if (attachedWeapons.Length != 0){
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
        else{
            Debug.LogWarning("No weapon assigned to mob! Defaulting to idle ai");
            
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
