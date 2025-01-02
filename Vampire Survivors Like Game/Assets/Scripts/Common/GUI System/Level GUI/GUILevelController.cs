using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;


public class GUILevelController : MonoBehaviour
{

    private LevelController lvlInstance;

    private TMP_Text mobCountDisplay;
    private TMP_Text waveCountDisplay;

    private TMP_Text playerScoreCountDisplay;
    private TMP_Text playerWeaponDisplay;
    private TMP_Text playerHealthDisplay;
    private TMP_Text playerExpDisplay;
    private TMP_Text playerTimeAliveDisplay;
    private TMP_Text currentDifficultyDisplay;

    private Texture2D miniMapCompoment;
    private MiniMapController miniMapScript;

    public GameObject death_panel;

    void Awake()
    {
        TMP_Text[] temp = transform.GetComponentsInChildren<TMP_Text>();

        for (int i = 0; i < temp.Length; i++){
            switch(temp[i].gameObject.name){
                case "mob_counter":
                    mobCountDisplay = temp[i];
                    break;
                case "wave_counter":
                    waveCountDisplay = temp[i];
                    break;
                case "health_counter":
                    playerHealthDisplay = temp[i];
                    break;    
                case "exp_counter":
                    playerExpDisplay = temp[i];
                    break;    
                case "playerweapons_counter":
                    playerWeaponDisplay = temp[i];
                    break;    
                case "score_counter":
                    playerScoreCountDisplay = temp[i];
                    break;    
                case "timealive_counter":
                    playerTimeAliveDisplay = temp[i];
                    break;     
                case "current_difficulty_counter":
                    currentDifficultyDisplay = temp[i];
                    break;               
            }
        }
    }

    void Update()
    {

        if (lvlInstance != null){
            if (lvlInstance.playerInstance.isDead == true && death_panel.activeSelf == false){
                death_panel.SetActive(true);
            }
            if (lvlInstance.playerInstance.isDead == false && death_panel.activeSelf == true){
                death_panel.SetActive(false);
            }

            mobCountDisplay.text = "Mob Count - " + lvlInstance.mobContainerObj.transform.childCount;
            currentDifficultyDisplay.text = "Current Difficulty: " + (lvlInstance.spawnChance - 1) +  "%";
            playerScoreCountDisplay.text = "Score - " + lvlInstance.playerInstance.score;
            playerTimeAliveDisplay.text = "Time Alive - " + (int)lvlInstance.playerInstance.timeAlive + " Sec";
            playerHealthDisplay.text = "Health: " + lvlInstance.playerInstance.GetHealth();
            waveCountDisplay.text = "Wave: " + lvlInstance.waveCount;
            

            IWeaponController[] weapons = lvlInstance.playerInstance.GetComponentsInChildren<IWeaponController>();

            string txt = "Weapons - \n";
            for (int i = 0; i < weapons.Length; i++){
                txt += weapons[i].internalName + "\n";

                switch(weapons[i].type){
                    case WeaponType.Spawner:
                        MinionSpawnController mWeapon = weapons[i] as MinionSpawnController;
                        txt += "Minion Count: " + mWeapon.spawnedMobs.Count + " / " + mWeapon.maxSpawnedMobs + "\n";
                        break;

                    case WeaponType.Projectile:
                        WeaponController pWeapon = weapons[i] as WeaponController;
                        txt += "Weapon Damage: " + pWeapon.projObj.damage + "\n";
                        break;
                    default:
                        break;
                }

                txt += "------------ \n";
            }

            playerWeaponDisplay.text = txt;
        }

    }

    public void setLevelInstance(LevelController lvlController){
        lvlInstance = lvlController;
    }
}
