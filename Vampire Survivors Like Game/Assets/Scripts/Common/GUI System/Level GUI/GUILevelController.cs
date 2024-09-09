using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class GUILevelController : MonoBehaviour
{
    private TMP_Text mobCountDisplay;
    private TMP_Text waveCountDisplay;
    private TMP_Text scoreCountDisplay;

    // Start is called before the first frame update
    void Start()
    {
        TMP_Text[] temp = transform.GetComponentsInChildren<TMP_Text>();

        for (int i = 0; i < temp.Length; i++){
            if (temp[i].gameObject.name == "mob_counter"){
                mobCountDisplay = temp[i];
            }
            if (temp[i].gameObject.name == "wave_counter"){
                waveCountDisplay = temp[i];
            }
                if (temp[i].gameObject.name == "score_counter"){
                scoreCountDisplay = temp[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        mobCountDisplay.text = "Mob Count - " + GameController.Instance.levelInstance.mobContainerObj.transform.childCount;
        scoreCountDisplay.text = "Score - " + GameController.Instance.levelInstance.playerInstance.score;
    }
}
