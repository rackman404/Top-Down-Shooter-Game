using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
        GUIMenuScript = Instantiate(GUIMenuControllerPrefab).GetComponent<GUIMenuController>();
        GUIMenuScript.transform.SetParent(this.transform);

        if (GameController.Instance.levelInstance != null){ //if game was started without being on main menu (i.e editor)
            GUIMenuScript.gameObject.SetActive(false);
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
}
