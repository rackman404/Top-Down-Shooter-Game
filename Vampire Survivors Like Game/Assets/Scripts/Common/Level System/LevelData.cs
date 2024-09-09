using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class LevelData
{  
    /// <summary>
    /// To be used for save file version control.
    /// </summary>
    public float saveVersion {get; private set;} = 1.0f;

    [JsonProperty("mobEntities")]
    public Dictionary<string, string>[] mobEntities {get; private set;}
    [JsonProperty("projectileEntities")]
    public Dictionary<string, string>[] projectileEntities {get; private set;}
    [JsonProperty("playerEntity")]
    public Dictionary<string, string> playerEntity {get; private set;}

    public void StoreAllEntityData(LevelController lvlController){
        mobEntities = new Dictionary<string, string>[lvlController.mobContainerObj.transform.childCount];
        projectileEntities = new Dictionary<string, string>[lvlController.projContainerObj.transform.childCount];
        playerEntity = new Dictionary<string, string>();

        if (lvlController.mobContainerObj.transform.childCount != 0){
            for (int i = 0; i < lvlController.mobContainerObj.transform.childCount; i++){
                Transform mobTemp = lvlController.mobContainerObj.transform.GetChild(i);
                MobEntity mobScript = mobTemp.GetComponent<MobEntity>();

                Dictionary<string, string> currentEnt = new Dictionary<string, string>
                {
                    { "transform x", mobTemp.position.x.ToString() },
                    { "transform y", mobTemp.position.y.ToString() },
                    { "health", mobScript.GetHealth().ToString() },
                    { "prefab name", mobScript.prefabName }
                };

                mobEntities[i] = currentEnt;
            }
        }

        if (lvlController.projContainerObj.transform.childCount != 0){
            for (int i = 0; i < lvlController.projContainerObj.transform.childCount; i++){
                Transform projTemp = lvlController.projContainerObj.transform.GetChild(i);
                ProjectileEntity projScript = projTemp.GetComponent<ProjectileEntity>();

                Dictionary<string, string> currentProj = new Dictionary<string, string>
                {
                    { "transform x", projTemp.position.x.ToString() },
                    { "transform y", projTemp.position.y.ToString() },
                    { "prefab name", projScript.prefabName },
                    { "direction vec x", projScript.directionVector.x.ToString()},
                    { "direction vec y", projScript.directionVector.y.ToString()},
                    { "origin tag", projScript.originObjTag }
                };

                projectileEntities[i] = currentProj;
            }
        }

        if(lvlController.playerInstance.isDead != true){
            playerEntity = new Dictionary<string, string>
            {
                { "transform x", lvlController.playerInstance.transform.position.x.ToString() },
                { "transform y", lvlController.playerInstance.transform.position.y.ToString() },
                { "health", lvlController.playerInstance.GetHealth().ToString() },
                { "score", lvlController.playerInstance.score.ToString()}
            };
        }
        
    }

}
