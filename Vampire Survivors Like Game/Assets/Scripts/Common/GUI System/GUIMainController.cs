using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GUIMainController : MonoBehaviour
{

    public GameObject GUILevelControllerPrefab;
    public GameObject GUIMenuControllerPrefab;

    private GUIMenuController GUIMenuScript;
    private GUILevelController GUILevelScript;

    void Start(){
        GUILevelScript = Instantiate(GUILevelControllerPrefab,this.transform).GetComponent<GUILevelController>();
        GUIMenuScript = Instantiate(GUIMenuControllerPrefab, this.transform).GetComponent<GUIMenuController>();
        
        GUIMenuScript.GUImainControl = this;

        if (GameController.Instance.levelInstance != null){ //if game was started without being on main menu (i.e editor)
            GUIMenuScript.gameObject.SetActive(false);
            GUIMenuScript.GameMenuMode();
        }
        else{
            GUIMenuScript.gameObject.SetActive(true);
            GUIMenuScript.MainMenuMode();
        }
    }

    // Update is called once per frame
    void Update()
    {
        GUILevelScript.setLevelInstance(GameController.Instance.levelInstance);

        if (GameController.Instance.levelInstance != null && GUILevelScript.gameObject.activeSelf == false){
            GUILevelScript.gameObject.SetActive(true);
        }
        else if (GameController.Instance.levelInstance == null && GUILevelScript.gameObject.activeSelf == true){
            GUILevelScript.gameObject.SetActive(false);
        }

        if (GameController.Instance.paused == false && GUIMenuScript.gameObject.activeSelf == true){
            GUIMenuScript.gameObject.SetActive(false);
        }
        if (GameController.Instance.paused == true && GUIMenuScript.gameObject.activeSelf == false){
            GUIMenuScript.gameObject.SetActive(true);
        }

    }

    //External event Listeners ------
    public void CallReset(){
        GameController.Instance.RestartGameState();
    }

    public void CallSave(){
        GameController.Instance.levelInstance.SaveLevelData();
    }

    public void CallLoad(){
        GameController.Instance.RestartGameState();
        GameController.Instance.levelInstance.LoadLevelData();
    }

    public void CallLoadMainMenu(){
        CallStartLevel();

        StartCoroutine(AsyncLoad());

        IEnumerator AsyncLoad(){

            while (GameController.Instance.levelInstance == null){
                yield return new WaitForSeconds(0.01f);
            }

            GameController.Instance.RestartGameState();
            GameController.Instance.levelInstance.LoadLevelData();
        }

    }

    public void CallResume(){
        GameController.Instance.Resume();
    }

    public void CallStartLevel(){
        GameController.Instance.RestartGameState();
        GUIMenuScript.GameMenuMode();
    }

    public void CallExitLevel(){
        GameController.Instance.ExitLevel();

        GUIMenuScript.MainMenuMode();
    }

    public void CallExitToDesktop(){
        Application.Quit();
    }
}
