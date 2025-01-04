using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TerrainType{
    Normal, LandImpassable, AllImpassable
}

public class TerrainManager : MonoBehaviour
{

    public Tilemap tileSet;
    public Grid grid;
    public TerrainData levelTerrainData;

    private List<Vector3Int> loadedChunkCoordinates = new List<Vector3Int>();

    private const int CHUNKCHECKRADIUS = 50;

    
    void Start()
    {

        //center grid on 0,0
        //grid.transform.position = new Vector3 (levelTerrainData.chunkSize/2 * grid.cellSize.x, levelTerrainData.chunkSize/2 * grid.cellSize.y);

        //initial chunk load
        LoadChunk(0,0);
    }


    //TEMP

    private int tickRun = 10;
    private int tickRunCounter = 0 ;

    /// <summary>
    /// Load new chunk based on given coordinates.
    /// </summary>
    private void LoadChunk(int chunkX, int chunkY){

        int chunkCenterX = chunkX * levelTerrainData.chunkSize;
        int chunkCenterY = chunkY * levelTerrainData.chunkSize;

        Vector3Int vec = Vector3Int.zero;
        for (int i = -levelTerrainData.chunkSize/2 + chunkCenterX; i < levelTerrainData.chunkSize/2 + chunkCenterX; i++){
            for (int j = -levelTerrainData.chunkSize/2 + chunkCenterY; j < levelTerrainData.chunkSize/2 + chunkCenterY; j++){

                vec.x = i;
                vec.y = j;
                if (tileSet.GetTile(vec) == null){
                    tileSet.SetTile(vec, levelTerrainData.tiles[0]);
                }

            }
        }

        loadedChunkCoordinates.Add(new Vector3Int(chunkX, chunkY));


        void NoiseGeneration(){

        }


    }

    /// <summary>
    /// Check if new chunks need to be loaded by checking if tiles are loaded just outside the range of player LOS.
    /// </summary>
    private void ChunkLoadCheck(){
        // Algorithm:
        // check player position + buffer radius for a null tile
        // if null:
        //      get chunk coordinate of given tile
        //      

        Vector3 playerPos = GameController.Instance.levelInstance.playerInstance.transform.position;

        //Debug.Log(ToChunkCoordinate(ToTileCoordinate(playerPos)).x + " " + ToChunkCoordinate(ToTileCoordinate(playerPos)).y);

        /*
        Vector3Int checkR = Vector3Int.RoundToInt(playerPos); checkR = ToTileCoordinate(checkR); checkR.x += CHUNKCHECKRADIUS;

        Vector3Int checkL = Vector3Int.RoundToInt(playerPos); checkL = ToTileCoordinate(checkL); checkL.x -= CHUNKCHECKRADIUS;
        Vector3Int checkU = Vector3Int.RoundToInt(playerPos); checkU = ToTileCoordinate(checkU); checkU.y += CHUNKCHECKRADIUS;
        Vector3Int checkD = Vector3Int.RoundToInt(playerPos); checkD = ToTileCoordinate(checkD); checkD.y -= CHUNKCHECKRADIUS;

        CheckInDirection(checkR);
        CheckInDirection(checkL);
        CheckInDirection(checkU);
        CheckInDirection(checkD);
        */

        
        for (int i = -CHUNKCHECKRADIUS; i != CHUNKCHECKRADIUS * 2; i += CHUNKCHECKRADIUS){
            for(int j = -CHUNKCHECKRADIUS; j != CHUNKCHECKRADIUS * 2; j += CHUNKCHECKRADIUS){
                Vector3Int checkTilePos = Vector3Int.RoundToInt(playerPos); checkTilePos = ToTileCoordinate(checkTilePos); checkTilePos.x += i; checkTilePos.y += j;
                CheckInDirection(checkTilePos);
            }
        }
        

        //Check and load chunks within the position around player given.
        void CheckInDirection(Vector3Int checkTilePos){
            Vector3Int chunkCoordinate = ToChunkCoordinate(checkTilePos);
            //Debug.Log("Chunk coordinate: " +  chunkCoordinate.x + " " + chunkCoordinate.y);
            //Debug.Log("Tile Check Position: " +  checkTilePos.x + " " + checkTilePos.y);
            if (tileSet.GetTile(checkTilePos) == null){

                if (loadedChunkCoordinates.Contains(chunkCoordinate) == false){
                    LoadChunk(chunkCoordinate.x, chunkCoordinate.y);
                }
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

        //retarded lookin aah code should kms
        if (Mathf.Sign(tileCoordinate.x) == 1){
            x = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.x)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.x + 50)/levelTerrainData.chunkSize)));
        }
        else{
            x = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.x)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.x - 50)/levelTerrainData.chunkSize)));
        }

        if (Mathf.Sign(tileCoordinate.y) == 1){
            y = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.y)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.y + 50)/levelTerrainData.chunkSize)));
        }
        else{
            y = Mathf.RoundToInt(Mathf.Sign(tileCoordinate.y)*Mathf.Floor(Mathf.Abs(((float)tileCoordinate.y - 50)/levelTerrainData.chunkSize)));
        }
        Vector3Int chunkCoordinate = new Vector3Int(
            x
        ,y
        
        );
        
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
        if (tickRunCounter == tickRun){
            ChunkLoadCheck();
            tickRunCounter = 0;
        }   

        
    }

}
