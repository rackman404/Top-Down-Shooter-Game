using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TerrainType{
    Normal, LandImpassable, AllImpassable
}

public enum NoiseGenerationType{
    None, CompleteRandom, Perlin
}

public enum FeatureType{
    Lake, Mountain, Generic
}

public class TerrainManager : MonoBehaviour
{
    public NoiseGenerationType noiseGenSetting;

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
        LoadChunk(0,0, new Vector3Int (0,0), true);
    }


    /// <summary>
    /// Load new chunk based on given coordinates.
    /// </summary>
    private void LoadChunk(int chunkX, int chunkY, Vector3Int chunkCoordinate, bool forceGenerateOnSingleFrame){


        int chunkCenterX = chunkX * levelTerrainData.chunkSize;
        int chunkCenterY = chunkY * levelTerrainData.chunkSize;

        Vector3Int vec = Vector3Int.zero;

        //only generate new strips at a time
        //Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        //TileBase[] tiles = new TileBase[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
        TileBase[] tiles = new TileBase[levelTerrainData.chunkSize];

        int counter;

        loadedChunkCoordinates.Add(chunkCoordinate);
        if (forceGenerateOnSingleFrame == false){
            StartCoroutine(ChunkGeneration(false));
        }
        else{
            StartCoroutine(ChunkGeneration(true));
        }
        
        IEnumerator ChunkGeneration(bool forceGenerate){
            for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
                tiles = new TileBase[levelTerrainData.chunkSize];
                counter = 0;

                for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                    NoiseGeneration(i,j);

                    //foreach loop used by the following: https://stackoverflow.com/questions/972307/how-to-loop-through-all-enum-values-in-c 
                    float featureOffsetCounter = FEATURENOISEOFFSET;
                    foreach(FeatureType foo in FeatureType.GetValues(typeof(FeatureType))){
                        if (foo != FeatureType.Generic){
                            FeatureGeneration(i,j, foo, featureOffsetCounter);
                            featureOffsetCounter += FEATURENOISEOFFSET;
                        }
                    }
                    
                    counter++;
                } 

                //generate a new strip of tiles before waiting for new frame to generate the next one
                tileSet.SetTiles(tileCoordinates, tiles);

                if (forceGenerate == false){//generate entire thing at once if force generate is false
                    yield return new WaitForEndOfFrame(); 
                }
            }
            yield return null;
        }

        /*
        1. Generates a noise value
        2. if there are tiles from the selected feature:
            3. select tile that meets the noise value threshold
            4. if a tile was chosen, place tile
        */
        void FeatureGeneration(int i, int j, FeatureType feature, float featureOffset){
            vec.x = i;
            vec.y = j;
            
            float value = Mathf.PerlinNoise(PERLINNOISEOFFSET + featureOffset + levelTerrainData.seedID + (float)i/10,PERLINNOISEOFFSET + featureOffset + levelTerrainData.seedID + (float)j/10) * 
            Mathf.PerlinNoise(PERLINNOISEOFFSET + featureOffset + levelTerrainData.seedID + (float)i/10,PERLINNOISEOFFSET + featureOffset + levelTerrainData.seedID + (float)j/10);
            int tileCounter = 0;
            bool tileChosen = false;
            for (int k = 0; k < levelTerrainData.tiles.Length; k++){
                if (feature == levelTerrainData.FeatureTypes[k]){
                    if (value > levelTerrainData.tileNoiseValue[k] / 100.0f){ // divide by 100.0f because tileNoiseValue is a int from 0-100 but value is from 0.0 - 1.0
                        tileCounter = k;
                        tileChosen = true;
                    }
                }
                if (tileChosen == true){
                    tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                    tiles[counter] = levelTerrainData.tiles[tileCounter];
                }

            }

        }

        void NoiseGeneration(int i, int j) //generate base terrain based on given noise settings
        {
            switch (noiseGenSetting){
                case NoiseGenerationType.None:
                        vec.x = i;
                        vec.y = j;
                        if (tileSet.GetTile(vec) == null){
                            tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                            tiles[counter] = levelTerrainData.tiles[0];
                        }
                    break;
                case NoiseGenerationType.Perlin:
                        vec.x = i;
                        vec.y = j;
                        if (tileSet.GetTile(vec) == null){
                            tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                            float value = Mathf.PerlinNoise(PERLINNOISEOFFSET + levelTerrainData.seedID + (float)i/10,PERLINNOISEOFFSET + levelTerrainData.seedID + (float)j/10);
                            if (value > 0.5){
                                tiles[counter] = levelTerrainData.tiles[0];
                            }
                            else{
                                tiles[counter] = levelTerrainData.tiles[1];
                            }
                        }
                    break;
                case NoiseGenerationType.CompleteRandom:
                            vec.x = i;
                            vec.y = j;
                            if (tileSet.GetTile(vec) == null){
                                int tileCount = levelTerrainData.tiles.Length;
                                tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                tiles[counter] = levelTerrainData.tiles[rng.Next(0,tileCount)];
                            }
                    break;
                default:
                    break;
            }
            
        }



    }

    /// <summary>
    /// unloads all tiles in the given chunk by setting them to null. uses SetTilesBlocks() for efficency
    /// </summary>
    private void UnloadChunk(int chunkX, int chunkY, Vector3Int chunkCoordinateObject){

        int chunkCenterX = chunkX * levelTerrainData.chunkSize;
        int chunkCenterY = chunkY * levelTerrainData.chunkSize;
        

        Vector3Int vec = Vector3Int.zero;
        //Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        //TileBase[] tiles = new TileBase[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
        TileBase[] tiles = new TileBase[levelTerrainData.chunkSize];

        int counter = 0;

        StartCoroutine(Unload());

        IEnumerator Unload(){
            for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
                tiles = new TileBase[levelTerrainData.chunkSize];
                counter = 0;
                for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                    vec.x = i;
                    vec.y = j;
                    
                    if (tileSet.GetTile(vec) != null){
                        tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                        counter++;
                    }
                }
                tileSet.SetTiles(tileCoordinates, tiles);
                yield return new WaitForEndOfFrame();
            }
        }

        loadedChunkCoordinates.Remove(chunkCoordinateObject);

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
                    LoadChunk(chunkCoordinate.x, chunkCoordinate.y, chunkCoordinate,  false);

                    
                }
            }
        }

    }

    /// <summary>
    /// Unloads chunks if too far.
    /// </summary>
    private void ChunkUnloadCheck(){

        for (int i = 0; i < loadedChunkCoordinates.Count; i++){
            if (nearbyChunkCoordinates.Contains(loadedChunkCoordinates[i]) == false){
                UnloadChunk(loadedChunkCoordinates[i].x, loadedChunkCoordinates[i].y, loadedChunkCoordinates[i]);
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
