using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TerrainTile 
{
    public TileBase tile;
    public FeatureType FeatureType;
    public TerrainType TerrainTypes;
    public int tileNoiseValue;
}

    