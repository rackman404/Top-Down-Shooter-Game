using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Tilemaps;


public enum TerrainType{
    Normal, LandImpassable, AllImpassable
}

public enum NoiseGenerationType{
    None, CompleteRandom, Perlin
}

public class TerrainManager : MonoBehaviour
{
    public NoiseGenerationType noiseGenSetting;
    private int seedID = 0;

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
    private int tickRunCounter = 0;

    void Start()
    {
        rng = new System.Random(seedID);
        ChunkLoadCheck();
    }


    /// <summary>
    /// Load new chunk based on given coordinates.
    /// </summary>
    private void LoadChunk(int chunkX, int chunkY){


        int chunkCenterX = chunkX * levelTerrainData.chunkSize;
        int chunkCenterY = chunkY * levelTerrainData.chunkSize;

        Vector3Int vec = Vector3Int.zero;

        //only generate new strips at a time
        //Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        //TileBase[] tiles = new TileBase[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
        TileBase[] tiles = new TileBase[levelTerrainData.chunkSize];


        int counter = 0;




        StartCoroutine(NoiseGeneration());
        

        IEnumerator NoiseGeneration() //attempt to generate a single strip of terrain from each chunk per frame to spread load out
        {
            switch (noiseGenSetting){
                case NoiseGenerationType.None:
                    for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                        tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
                        tiles = new TileBase[levelTerrainData.chunkSize];
                        counter = 0;
                        for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                            vec.x = i;
                            vec.y = j;
                            if (tileSet.GetTile(vec) == null){
                                tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                tiles[counter] = levelTerrainData.tiles[0];
                                counter++;
                            }
                        }
                        tileSet.SetTiles(tileCoordinates, tiles);
                        yield return new WaitForEndOfFrame();
                    } 
                    
                    break;
                case NoiseGenerationType.Perlin:
                    for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                        tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
                        tiles = new TileBase[levelTerrainData.chunkSize];
                        counter = 0;
                        for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                            vec.x = i;
                            vec.y = j;
                            if (tileSet.GetTile(vec) == null){
                                tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                float value = Mathf.PerlinNoise(100000.0f + (float)i/10,100000.0f + (float)j/10);
                                if (value > 0.5){
                                    tiles[counter] = levelTerrainData.tiles[0];
                                }
                                else{
                                    tiles[counter] = levelTerrainData.tiles[1];
                                }
                                counter++;
                            }
                        }
                        tileSet.SetTiles(tileCoordinates, tiles);
                        yield return new WaitForEndOfFrame();
                    } 

                    break;
                case NoiseGenerationType.CompleteRandom:
                    for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                        tileCoordinates = new Vector3Int[levelTerrainData.chunkSize];
                        tiles = new TileBase[levelTerrainData.chunkSize];
                        counter = 0;
                        for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                            vec.x = i;
                            vec.y = j;
                            
                            if (tileSet.GetTile(vec) == null){
                                int tileCount = levelTerrainData.tiles.Length;
                                tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                tiles[counter] = levelTerrainData.tiles[rng.Next(0,tileCount)];
                                counter++;
                            }
                        }
                        tileSet.SetTiles(tileCoordinates, tiles);
                        yield return new WaitForEndOfFrame();
                    }

                    break;
                default:
                    break;
            }
            
        }

        /* without spreading generation across multiple frames
        NoiseGeneration();
        void NoiseGeneration(){
            switch (noiseGenSetting){
                case NoiseGenerationType.None:
                    for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                        for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){

                            vec.x = i;
                            vec.y = j;
                            if (tileSet.GetTile(vec) == null){
                                tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                tiles[counter] = levelTerrainData.tiles[0];
                                counter++;
                            }
                        }
                    } 
                    break;
                case NoiseGenerationType.Perlin:
                    for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                        for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){

                            vec.x = i;
                            vec.y = j;
                            if (tileSet.GetTile(vec) == null){
                                tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                float value = Mathf.PerlinNoise(100000.0f + (float)i/10,100000.0f + (float)j/10);
                                if (value > 0.5){
                                    tiles[counter] = levelTerrainData.tiles[0];
                                }
                                else{
                                    tiles[counter] = levelTerrainData.tiles[1];
                                }
                                counter++;
                            }
                        }
                    } 

                    break;
                case NoiseGenerationType.CompleteRandom:

                    int tileCount = levelTerrainData.tiles.Length;

                    for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
                        for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                            vec.x = i;
                            vec.y = j;
                            
                            if (tileSet.GetTile(vec) == null){
                                tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                                tiles[counter] = levelTerrainData.tiles[rng.Next(0,tileCount)];
                                counter++;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        */




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

    /*without coroutines

        int chunkCenterX = chunkX * levelTerrainData.chunkSize;
        int chunkCenterY = chunkY * levelTerrainData.chunkSize;
        

        Vector3Int vec = Vector3Int.zero;
        Vector3Int[] tileCoordinates = new Vector3Int[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        TileBase[] tiles = new TileBase[levelTerrainData.chunkSize * levelTerrainData.chunkSize];
        int counter = 0;

        for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
            for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){
                vec.x = i;
                vec.y = j;
                
                if (tileSet.GetTile(vec) != null){
                    tileCoordinates[counter] = new Vector3Int(vec.x, vec.y);
                    counter++;
                }
            }
        }

        tileSet.SetTiles(tileCoordinates, tiles);

        loadedChunkCoordinates.Remove(chunkCoordinateObject);

    */
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
                    LoadChunk(chunkCoordinate.x, chunkCoordinate.y);
                    loadedChunkCoordinates.Add(chunkCoordinate);
                    
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
