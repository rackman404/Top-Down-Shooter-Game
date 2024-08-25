using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    [JsonProperty("mobEntities")]
    public Dictionary<string, string>[] mobEntities {get; private set;}
    [JsonProperty("projectileEntities")]
    public Dictionary<string, string>[] projectileEntities {get; private set;}
    [JsonProperty("playerEntity")]
    public Dictionary<string, string> playerEntity {get; private set;}

    public void StoreAllEntityData(GameController gameController){
        mobEntities = new Dictionary<string, string>[gameController.mobContainerObj.transform.childCount];
        projectileEntities = new Dictionary<string, string>[gameController.projContainerObj.transform.childCount];
        playerEntity = new Dictionary<string, string>();

        if (gameController.mobContainerObj.transform.childCount != 0){
            for (int i = 0; i < gameController.mobContainerObj.transform.childCount; i++){
                Transform mobTemp = gameController.mobContainerObj.transform.GetChild(i);
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

        if (gameController.projContainerObj.transform.childCount != 0){
            for (int i = 0; i < gameController.projContainerObj.transform.childCount; i++){
                Transform projTemp = gameController.projContainerObj.transform.GetChild(i);
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

        if(gameController.playerInstance.isDead != true){
            playerEntity = new Dictionary<string, string>
            {
                { "transform x", gameController.playerInstance.transform.position.x.ToString() },
                { "transform y", gameController.playerInstance.transform.position.y.ToString() },
                { "health", gameController.playerInstance.GetHealth().ToString() }
            };
        }
        
    }

}
