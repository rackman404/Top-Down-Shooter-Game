using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Terrain Data", menuName = "ScriptableObjects/TerrainData", order = 1)]
public class TerrainData : ScriptableObject
{

    [Header("Tileset Parameters")]
    public string tileSetName;

    public int chunkSize;

    public TileBase[] tiles;

    public int[] tileSpawnChance;

    private int tilesCount;

    void OnValidate(){
        if (tilesCount != tiles.Length){
            tileSpawnChance = new int[tiles.Length];
            tilesCount = tiles.Length;
        }
        
    }



}
