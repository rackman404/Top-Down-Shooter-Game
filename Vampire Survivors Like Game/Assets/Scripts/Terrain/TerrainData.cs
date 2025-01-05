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


    public string tileSetName;

    public bool randomSeed;

    [Header("use if seed is not randomly generated")]
    public int seedID;

    public int chunkSize;

    public TileBase[] tiles;

    [Header("Tileset Parameters")]
    public int[] tileNoiseValue;

    public NoiseGenerationType[] FeatureGenerationTypes;

    public FeatureType[] FeatureTypes;

    public TerrainType[] TerrainTypes;

    private int tilesCount;

    void OnValidate(){
        if (tilesCount != tiles.Length){
            Array.Resize<NoiseGenerationType>(ref FeatureGenerationTypes, Enum.GetNames(typeof(NoiseGenerationType)).Length);

            Array.Resize<int>(ref tileNoiseValue, tiles.Length);
            Array.Resize<FeatureType>(ref FeatureTypes, tiles.Length);
            Array.Resize<TerrainType>(ref TerrainTypes, tiles.Length);

            tilesCount = tiles.Length;
        }

        
    }



}
