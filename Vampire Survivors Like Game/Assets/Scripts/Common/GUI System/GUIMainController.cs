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

    private bool menuOn = false;


    void Start(){
        GUILevelScript = Instantiate(GUILevelControllerPrefab,this.transform).GetComponent<GUILevelController>();
        GUILevelScript.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3 (550,291,0), Quaternion.Euler(0,0,0));
        GUIMenuScript = Instantiate(GUIMenuControllerPrefab).GetComponent<GUIMenuController>();
        GUIMenuScript.transform.SetParent(this.transform);

        if (GameController.Instance.levelInstance != null){ //if game was started without being on main menu (i.e editor)
            GUIMenuScript.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (menuOn == false && GUIMenuScript.gameObject.activeSelf == true){
            GUIMenuScript.gameObject.SetActive(false);
        }
    }
}
