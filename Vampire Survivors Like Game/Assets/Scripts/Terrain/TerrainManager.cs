using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Codice.Client.BaseCommands;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TerrainType{
    Normal, LandImpassable, AllImpassable
}

public enum NoiseGenerationType{
    None, CompleteRandom, Perlin
}

/// <summary>
/// Different ways to divide the chunk generation work between frames
/// </summary>
public enum ChunkLoadingType{
    Strip, Random
}

//public enum FeatureType{
//    Lake, Mountain, Generic
//}

public class TerrainManager : MonoBehaviour
{

    public ChunkLoadingType chunkLoadType;


    private System.Random rng;
    public Tilemap tileSet;
    public Grid grid;
    public TerrainData levelTerrainData;

    /// <summary>
    /// Chunks that are currently loadedd
    /// </summary>
    private List<Vector3Int> loadedChunkCoordinates = new List<Vector3Int>();

    /// <summary>
    /// For chunks that are to never be unloaded after being loaded.
    /// </summary>
    private List<Vector3Int> persistantChunkCoordinates = new List<Vector3Int>();
    
    /// <summary>
    /// For chunks that are nearby the player.
    /// </summary>
    private List<Vector3Int> nearbyChunkCoordinates = new List<Vector3Int>();

    private List<List<int>> intArray = new List<List<int>>();

    //should be 3/4 of the chunk size
    private readonly int CHUNKCHECKRADIUS = 100;
    private bool chunkLoadCoroutineActive = false;

    private const int TICKRUN = 50;
    private const float PERLINNOISEOFFSET = 10000.0f;
    private const float FEATURENOISEOFFSET = 10.0f;

    private int tickRunCounter = 0;

    void Start()
    {
        rng = new System.Random(levelTerrainData.seedID);

        //initial chunk load 
        LoadChunk(0,0, new Vector3Int (0,0), true, chunkLoadType, false);
    }


    /// <summary>
    /// Load new chunk based on given coordinates.
    /// </summary>
    private void LoadChunk(int chunkX, int chunkY, Vector3Int chunkCoordinate, bool forceGenerateOnSingleFrame, ChunkLoadingType chunkLoadType, bool unload){

        int chunkCenterX = chunkX * levelTerrainData.chunkSize;
        int chunkCenterY = chunkY * levelTerrainData.chunkSize;

        Vector3Int vec = Vector3Int.zero;

        //only generate new strips at a time
        Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
        TileBase[] tiles = new TileBase[levelTerrainData.chunkSize];

        int counter;

        if (unload == false){
            loadedChunkCoordinates.Add(chunkCoordinate);
        }
        

        StartCoroutine(ChunkGeneration(forceGenerateOnSingleFrame, chunkLoadType));


        
        IEnumerator ChunkGeneration(bool forceGenerate, ChunkLoadingType chunkLoadType){


            switch (chunkLoadType){
                case ChunkLoadingType.Strip:
                        for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                            tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
                            tiles = new TileBase[levelTerrainData.chunkSize];
                            counter = 0;

                            for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                                //NoiseGeneration(i,j);
                                    
                                for (int k = 0; k < levelTerrainData.featureGenerationOrder.Length; k++){
                                    vec.x = i;
                                    vec.y = j;
                                    //Debug.Log(levelTerrainData.featureGenerationOrder[k].featureName);
                                    if (unload == false){
                                        FeatureGeneration(vec.x, vec.y, levelTerrainData.featureGenerationOrder[k], counter); //generate feature in tile of coordinate i,j
                                    }
                                    else{
                                        if (tileSet.GetTile(vec) != null){
                                            tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                        }
                                    }
                                }
                                 
                               counter++;
                            } 

                            //generate a new strip of tiles before waiting for new frame to generate the next one    
                            tileSet.SetTiles(tileCoordinates, tiles);
                            
                            


                            if (forceGenerate == false){//yields main thread to unity
                                yield return new WaitForEndOfFrame(); 
                            }
                        }

                    break;
                case ChunkLoadingType.Random:
                        List<Vector2Int> chunkTiles = new List<Vector2Int>();
                        for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                            for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                                chunkTiles.Add(new Vector2Int(i,j));
                            }
                        }

                        while (chunkTiles.Count != 0 ){
                            tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
                            tiles = new TileBase[levelTerrainData.chunkSize];
                            counter = 0;
                            
                            for (int i = 0; i < levelTerrainData.chunkSize; i++){

                                int tileSelected = rng.Next(0, chunkTiles.Count);      
                                    

                                for (int k = 0; k < levelTerrainData.featureGenerationOrder.Length; k++){
                                    vec.x = chunkTiles[tileSelected].x;
                                    vec.y = chunkTiles[tileSelected].y;

                                    if (unload == false){
                                        FeatureGeneration(vec.x, vec.y, levelTerrainData.featureGenerationOrder[k], counter); //generate feature in tile of coordinate i,j
                                    }
                                    else{
                                        if (tileSet.GetTile(vec) != null){
                                            tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                        }
                                    }
                                        
                                }

                                chunkTiles.RemoveAt(tileSelected);
                                counter++;  
                            }
                            
                            /*
                            if (chunkX == 0 && chunkY == 0){
                                Debug.Log(tileCoordinates[index].x + " " +  tileCoordinates[index].y);
                            }
                            */

                            tileSet.SetTiles(tileCoordinates, tiles);
                            
    
                            yield return new WaitForEndOfFrame(); 
                        }
                    break;
                default:
                    break;

            }

            if (unload == true){
                loadedChunkCoordinates.Remove(chunkCoordinate);
            }

            yield return null;
        }

        /*
        1. Generates a noise value
        2. if there are tiles from the selected feature:
            3. select tile that meets the noise value threshold
            4. if a tile was chosen, place tile
        */
        void FeatureGeneration(int i, int j, FeatureType feature, int index){

            float value = NoiseGeneration(i,j, feature.featureSeedID, feature.noiseGenSetting);
            int tileCounter = 0;
            bool tileChosen = false;

            for (int k = 0; k < levelTerrainData.tileDatas.Length; k++){
                if (feature.name == levelTerrainData.tileDatas[k].FeatureType.name){
                    if (value > levelTerrainData.tileDatas[k].tileNoiseValue / 100.0f){ // divide by 100.0f because tileNoiseValue is a int from 0-100 but value is from 0.0 - 1.0
                        tileCounter = k;
                        tileChosen = true;

                        //tileSet.SetTile(new Vector3Int(vec.x, vec.y),  levelTerrainData.tileDatas[tileCounter].tile);
                    }
                }
                if (tileChosen == true){
                    tileCoordinates[index] = new Vector3Int(i, j);

                    
                    //if (chunkX == 0 && chunkY == 0){
                    //    Debug.Log(tileCoordinates[index].x + " " +  tileCoordinates[index].y);
                    //}
                    

                    tiles[index] = levelTerrainData.tileDatas[tileCounter].tile;
                }
            }
            ///default
            //tileCoordinates[index] = new Vector3Int(i, j);
            //tiles[index] = levelTerrainData.tileDatas[0].tile;
        }

        float NoiseGeneration(int i, int j, float offset, NoiseGenerationType noiseGenSetting) 
        {
            float noiseValue;
            switch (noiseGenSetting){
                case NoiseGenerationType.None: //always return noise of 0.00f
                    noiseValue = 0.00f;
                    break;
                case NoiseGenerationType.Perlin: // psuedo random from 0.00f to 1.00f
                    noiseValue = Mathf.PerlinNoise(PERLINNOISEOFFSET + offset + levelTerrainData.seedID + (float)i/10,PERLINNOISEOFFSET + offset + levelTerrainData.seedID + (float)j/10) * 
                    Mathf.PerlinNoise(PERLINNOISEOFFSET + offset + levelTerrainData.seedID + (float)i/10,PERLINNOISEOFFSET + offset + levelTerrainData.seedID + (float)j/10);
                    break;
                case NoiseGenerationType.CompleteRandom: //completely random from 0.00f to 1.00f
                    noiseValue = rng.Next(1,101) / 100.0f;
                    break;
                default:
                    Debug.LogWarning("NoiseGen Warning");
                    return 0;
            }

            return noiseValue;
        }
    }

    /// <summary>
    /// Check if new chunks need to be loaded.
    /// </summary>
    private void ChunkLoadCheck(){
        // Chunk loading Algorithm:
        // check player position + given buffer radius for a null tile by checking a single tile in each 8 cardinal directions (plus player position itself)
        // if null:
        //      get chunk coordinate of given tile
        //      check if chunk is loaded
        //      if not loaded:
        //          Load chunk
        // else:
        //      do nothing


        Vector3 playerPos = GameController.Instance.levelInstance.playerInstance.transform.position;
        //Vector3 playerPos = Vector3.zero;

        for (int i = -CHUNKCHECKRADIUS; i != CHUNKCHECKRADIUS * 2; i += CHUNKCHECKRADIUS){
            for(int j = -CHUNKCHECKRADIUS; j != CHUNKCHECKRADIUS * 2; j += CHUNKCHECKRADIUS){
                Vector3Int checkTilePos = Vector3Int.RoundToInt(playerPos); checkTilePos = ToTileCoordinate(checkTilePos); checkTilePos.x += i; checkTilePos.y += j;
                CheckInDirection(checkTilePos);
            }
        }      

        //Check and load chunk from given tile coordinate
        void CheckInDirection(Vector3Int checkTilePos){
            Vector3Int chunkCoordinate = ToChunkCoordinate(checkTilePos);
            nearbyChunkCoordinates.Add(chunkCoordinate);
            //Debug.Log("Chunk coordinate: " +  chunkCoordinate.x + " " + chunkCoordinate.y);
            //Debug.Log("Tile Check Position: " +  checkTilePos.x + " " + checkTilePos.y);
            if (tileSet.GetTile(checkTilePos) == null){

                if (loadedChunkCoordinates.Contains(chunkCoordinate) == false){
                    LoadChunk(chunkCoordinate.x, chunkCoordinate.y, chunkCoordinate, false, chunkLoadType, false);

                    
                }
            }
        }

    }

    /// <summary>
    /// Unloads chunks if too far.
    /// </summary>
    private void ChunkUnloadCheck(){

        for (int i = 0; i < loadedChunkCoordinates.Count; i++){
            if (nearbyChunkCoordinates.Contains(loadedChunkCoordinates[i]) == false || persistantChunkCoordinates.Contains(loadedChunkCoordinates[i]) == true){
                LoadChunk(loadedChunkCoordinates[i].x, loadedChunkCoordinates[i].y, loadedChunkCoordinates[i], false, chunkLoadType, true);
                //UnloadChunk(loadedChunkCoordinates[i].x, loadedChunkCoordinates[i].y, loadedChunkCoordinates[i]);
            }
        }

    }

    /// <summary>
    /// convert a tile space coordinate to a chunk space coordinate
    /// </summary>
    /// <param name="tileCoordinate"></param>
    public Vector3Int ToChunkCoordinate(Vector3Int tileCoordinate){
       // Debug.Log((tileCoordinate.x + 50 )+ " " + levelTerrainData.chunkSize + " " + Mathf.Sign(tileCoordinate.y + 50) * Mathf.Abs(((float)tileCoordinate.y)/levelTerrainData.chunkSize));

        int x = 0;
        int y = 0;

        int CHUNKBUFFER = levelTerrainData.chunkSize/2;

        //RETARDED AAH LOOKIN CODE KMS 
        if (Mathf.Sign(tileCoordinate.x) == 1){
            x = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.x)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.x + CHUNKBUFFER)/levelTerrainData.chunkSize)));
        }
        else{
            x = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.x)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.x - CHUNKBUFFER)/levelTerrainData.chunkSize)));
        }

        if (Mathf.Sign(tileCoordinate.y) == 1){
            y = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.y)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.y + CHUNKBUFFER)/levelTerrainData.chunkSize)));
        }
        else{
            y = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.y)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.y - CHUNKBUFFER)/levelTerrainData.chunkSize)));
        }
        Vector3Int chunkCoordinate = new Vector3Int(x,y);
        
        //Debug.Log(tileCoordinate.x + " " + levelTerrainData.chunkSize);

        return chunkCoordinate;
    }

    /// <summary>
    /// convert a world space coordinate to a tile space coordinate
    /// </summary>
    public Vector3Int ToTileCoordinate (Vector3 worldCoordinate){
        //Vector3Int tilePos = Vector3Int.FloorToInt(playerPos); tilePos.x /= (int)grid.cellSize.x; tilePos.y /= (int)grid.cellSize.x;
        Vector3Int convertedTileCoordinate = Vector3Int.FloorToInt(worldCoordinate);
        convertedTileCoordinate.x /= (int)grid.cellSize.x;
        convertedTileCoordinate.y /= (int)grid.cellSize.y;
        
        return convertedTileCoordinate;
        
    }

    void FixedUpdate(){
        tickRunCounter += 1;
        if (tickRunCounter == TICKRUN){
            ChunkLoadCheck();
            ChunkUnloadCheck();
            nearbyChunkCoordinates.Clear();
            tickRunCounter = 0;
        }   
    }

}
