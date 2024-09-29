using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    [Header("audio parameters")]
    private AudioClip mainMenuBGM;
    private AudioClip gameOverSFX;

    AudioSource globalAudioSource;

    bool mainMenuPlaying = false;
    
    public static SoundManager Instance {get; private set;}
    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

        globalAudioSource = GetComponent<AudioSource>();
        globalAudioSource.loop = false;
        

        mainMenuBGM = Resources.Load<AudioClip>("Sound/698282__main_theme");
        gameOverSFX = Resources.Load<AudioClip>("Sound/133283__game-over");
    }

    void Update(){
        if (GameController.Instance.levelInstance == null){
            if (mainMenuPlaying == false){
                StartCoroutine (BeginMainMenuBGM());
            }
        }
        else if (globalAudioSource.isPlaying == true && mainMenuPlaying == true){
            StopMainMenuBGM();
        }
    }

    IEnumerator BeginMainMenuBGM(){
        mainMenuPlaying = true;
        globalAudioSource.loop = true;
        Debug.Log("main menu theme playing now");
        globalAudioSource.clip = mainMenuBGM;
        globalAudioSource.Play(0);
        yield return new WaitForEndOfFrame();
    }

    public void StopMainMenuBGM(){
        mainMenuPlaying = false;
        globalAudioSource.loop = false;
        globalAudioSource.clip = null;
        globalAudioSource.Stop();
    }

    public void BeginGameOverSFX(){
        globalAudioSource.clip = gameOverSFX;
        globalAudioSource.Play(0);
    }
}
