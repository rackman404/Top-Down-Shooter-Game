using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System;
using Unity.VisualScripting;

public class ImageReadout : MonoBehaviour{

    private string jsonAIFilePath;

    private int frameCounter;

    void Awake(){
        jsonAIFilePath = Application.persistentDataPath + "/frames.json";
    }

    private Camera camOutput;
    public RenderTexture camTexture;

    private bool threadJSONRunning = false;

    private bool shouldResetStruct = false;

    private int framesRecorded = 0;

    private const int MaxFRAMESRECORDED = 4;
    private const int PERFRAMERECORDED = 13;

    struct JSONData{
        public List<int[]> frameByteData;

        public int health;

        public float cEnemyDist; // closest enemy distance

        public float pScore;

        public JSONData(int hp, float cEDist, float pS){
            frameByteData = new List<int[]>();
            health = hp;    
            cEnemyDist = cEDist; 
            pScore = pS;
        }
    }

    JSONData jsonData;

    void Start(){
        camOutput = new GameObject("aicam").AddComponent<Camera>();
        camOutput.targetTexture = camTexture;
        camOutput.orthographic = true;
        camOutput.depth = -1;
        camOutput.orthographicSize = 50f;
        camOutput.clearFlags = CameraClearFlags.SolidColor;

        jsonData = new JSONData(GameController.Instance.levelInstance.playerInstance.GetHealth(), GetClosestEnemyDist(), GameController.Instance.levelInstance.playerInstance.score);
    }


    void FixedUpdate(){
        if (GameController.Instance.levelInstance.playerInstance != null){
           camOutput.transform.position = GameController.Instance.levelInstance.playerInstance.transform.position;
           camOutput.transform.position += new Vector3(0,0,-1f);
        }
        

        if (frameCounter == PERFRAMERECORDED){
            frameCounter = 0;
            RecordFrameData();
            framesRecorded++;
        }


        frameCounter++;

    }


    void Update(){
        
        if (shouldResetStruct == true){
            shouldResetStruct = false;
            jsonData = new JSONData(GameController.Instance.levelInstance.playerInstance.GetHealth(), GetClosestEnemyDist(), GameController.Instance.levelInstance.playerInstance.score);
        }
       


        if (threadJSONRunning == false && framesRecorded >= MaxFRAMESRECORDED){
                framesRecorded = 0;

                //https://discussions.unity.com/t/threading-in-unity/30663/2 
                //NOTE, threadjsonrunning bool variable is kinda needed
                // threads can't read write to same variable at the same time or else funky stuff happens
                var task = new System.Threading.Thread(ToJSON);
                task.Start();           
        }
    }

    private void RecordFrameData(){
        //https://docs.unity3d.com/ScriptReference/RenderTexture-active.html
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = camTexture;
        
        Texture2D tex = new Texture2D(camTexture.width, camTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0,0, tex.width, tex.height), 0, 0);
        tex.Apply();

        Color32[] data = tex.GetRawTextureData<Color32>().ToArray();
        if (threadJSONRunning == false){
            int[] temp = new int[data.Length];

            for (int i = 0; i < data.Length; i++){
                temp[i] = (int)new Color(data[i].r, data[i].g, data[i].b).grayscale;
            }

            jsonData.frameByteData.Add(temp);
        }
        
        RenderTexture.active = currentActiveRT;
        Destroy(tex);
    }

    private void ToJSON(){
        threadJSONRunning = true;
        Debug.Log("thread running");

        JsonSerializer serializer = new JsonSerializer();

        StreamWriter sw = new StreamWriter(jsonAIFilePath);
        JsonWriter writer = new JsonTextWriter(sw);
        
        //writer.Formatting = Formatting.Indented;
        serializer.Serialize(writer, jsonData);
        sw.Close();

        /*
        StreamReader sr = new StreamReader(jsonAIFilePath);
        JsonReader reader = new JsonTextReader(sr);
        List<int[]> tes = (List<int[]>)serializer.Deserialize(reader, typeof(List<int[]>));
        Debug.Log(tes[1][2]);
        */
        
        Debug.Log("thread stopped");
        threadJSONRunning = false;

        //reset list
        shouldResetStruct = true;
        
    }



    private float GetClosestEnemyDist(){
        GameObject[] mobs = new GameObject[GameController.Instance.levelInstance.transform.childCount];

        if (mobs.Length != 0){
            for (int i = 0; i <  GameController.Instance.levelInstance.transform.childCount; i++){
                mobs[i] =  GameController.Instance.levelInstance.transform.GetChild(i).gameObject;
            }


            GameObject leastDistObj = mobs[0];
            float leastDist = Int32.MaxValue;
            for (int i = 0; i < mobs.Length; i++){
                float dist =  Vector3.Distance(mobs[i].transform.position, transform.position);
                if (dist < leastDist){
                    leastDist = dist;
                    leastDistObj = mobs[i];
                }
            }

            return leastDist;
        }
        return -1f; //if no entities
    }


}
