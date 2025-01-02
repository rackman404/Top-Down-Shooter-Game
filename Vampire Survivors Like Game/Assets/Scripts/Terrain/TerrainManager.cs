using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TerrainType{
    grass, water
}

public class TerrainManager : MonoBehaviour
{

    public Tilemap tileSet;
    public Grid grid;
    public TerrainData levelTerrainData;

    private List<Vector2> loadedChunkCoordinates;

    private const int CHUNKCHECKRADIUS = 50;

    void Start()
    {
        //initial chunk load
        LoadChunk(0,0);
    }

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

        loadedChunkCoordinates.Add(new Vector2(chunkX, chunkY));
    }

    /// <summary>
    /// Check if new chunks need to be loaded by checking if tiles are loaded just outside the range of player LOS.
    /// </summary>
    private void ChunkLoadCheck(){
        Vector3 position = GameController.Instance.levelInstance.playerInstance.transform.position;

        Vector3Int tempR = Vector3Int.RoundToInt(position); tempR.x += CHUNKCHECKRADIUS;
        Vector3Int tempL = Vector3Int.RoundToInt(position); tempL.x -= CHUNKCHECKRADIUS;
        Vector3Int tempU = Vector3Int.RoundToInt(position); tempU.y += CHUNKCHECKRADIUS;
        Vector3Int tempD = Vector3Int.RoundToInt(position); tempD.y -= CHUNKCHECKRADIUS;

        //check right
        if (tileSet.GetTile(tempR) == null){

        }
        //check left
        else if (tileSet.GetTile(tempL) == null){

        }
        //check up
        else if (tileSet.GetTile(tempU) == null){

        }
        //check down
        else if (tileSet.GetTile(tempD) == null){

        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileCoordinate"></param>
    private void GetChunkCoordinate(Vector3Int tileCoordinate){

    }

    void FixedUpdate(){
        ChunkLoadCheck();


    }

}
