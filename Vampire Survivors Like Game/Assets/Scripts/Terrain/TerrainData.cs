using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;



[CreateAssetMenu(fileName = "Terrain Data", menuName = "ScriptableObjects/TerrainData", order = 1)]
public class TerrainData : ScriptableObject
{
    //editor set
    public string tileSetName;

    [Header("seed parameters")]
    public bool randomSeed;

    [Header("- use if seed is not randomly generated")]
    public int seedID;
    [Header("---------")]

    public int chunkSize;

    public TileBase[] tiles;

    public FeatureType[] featureGenerationOrder;

    public TerrainTile[] tileDatas;

    [Header("Tileset Parameters")]
    //public int[] tileNoiseValue;

    //public NoiseGenerationType[] FeatureGenerationTypes;

    //public FeatureType[] FeatureTypes;

    //public TerrainType[] TerrainTypes;

    [Header("---------")]

    private int tilesCount;

    void OnValidate(){
        if (tilesCount != tiles.Length){
           // Array.Resize<NoiseGenerationType>(ref FeatureGenerationTypes, Enum.GetNames(typeof(NoiseGenerationType)).Length);

           // Array.Resize<int>(ref tileNoiseValue, tiles.Length);
           // Array.Resize<FeatureType>(ref FeatureTypes, tiles.Length);
            //Array.Resize<TerrainType>(ref TerrainTypes, tiles.Length);

            tilesCount = tiles.Length;
        }

        
    }

    void Awake(){
        
        //EditorTileConvert();
    }

    /*
    //convert base tiles class used by generic unity tiles to my own defined tile class
    private void EditorTileConvert(){
        tileDatas = new TerrainTile[tiles.Length];

        for (int i = 0; i < tiles.Length; i++){ //copy data over with new polymorphed class
            tileDatas[i] = (TerrainTile)tiles[i]; 
        }
    }
    */


}
