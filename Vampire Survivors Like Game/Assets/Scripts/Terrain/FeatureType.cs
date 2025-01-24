using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature Type", menuName = "ScriptableObjects/FeatureType", order = 2)]
public class FeatureType : ScriptableObject

{   
    public string featureName;
    public int featureSeedID;
    public NoiseGenerationType noiseGenSetting;
}
