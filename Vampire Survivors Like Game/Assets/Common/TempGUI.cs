using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGUI : MonoBehaviour
{

    List<Rect> entityRects = new List<Rect>();
    List<string> entityHp = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
        entityRects = new List<Rect>();
        entityHp = new List<string>();

        if (GameController.Instance.gameState == true){
            CharacterEntity[] entities = GameObject.FindObjectsOfType<CharacterEntity>();
            for (int i = 0; i < entities.Length; i ++){
                GUIRectWithObject(entities[i].gameObject);
                entityHp.Add(entities[i].GetComponent<CharacterEntity>().GetHealth().ToString());
            }
        }
        
    }

     void OnGUI(){
        var borderSize = 1; // Border size in pixels
        var style = new GUIStyle();
        //Initialize RectOffset object
        style.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
        style.normal.background = Resources.Load<Texture2D>("GUI/BoundingBox");

        if (GameController.Instance.gameState == true){
            for (int i = 0; i < entityRects.Count; i++){
                //GUI.Box(unitHighlights[i], GUIContent.none, style);
                GUI.Label(new Rect(entityRects[i].xMin, entityRects[i].yMax, entityRects[i].xMin + 20,entityRects[i].yMax - 20), "HP:" + entityHp[i]);
            }
        }
        else{
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, Screen.width / 2 + 50, Screen.height + 50), "Gameover");
        }
        
    }

    public void GUIRectWithObject(GameObject go) {
        Vector3 cen = go.GetComponent<BoxCollider2D>().bounds.center;
        Vector3 ext = go.GetComponent<BoxCollider2D>().bounds.extents;
        Vector3[] extentPoints = new Vector3[8]
          {
              Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
              Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
              Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
              Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
              Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
              Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
              Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
              Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
          };
         Vector3 min = extentPoints[0];
         Vector3 max = extentPoints[0];
         foreach (Vector3 v in extentPoints)
         {
             min = Vector3.Min(min, v);
             max = Vector3.Max(max, v);
         }

        
        Vector3 viewPos = Camera.main.WorldToViewportPoint(go.transform.position);
        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0) // Your object is in the range of the camera, you can apply your behaviour
        {
            if (min.x < 0 || max.y < 0 || min.y < 0 || max.x < 0){
                entityRects.Add(new Rect(0,0,0,0));
                
            }
            else{
                entityRects.Add(new Rect(min.x, Screen.height - max.y  , max.x-min.x, max.y-min.y));
            }
            
        }
        else{
            entityRects.Add(new Rect(0,0,0,0));
        }

       
    }
}
