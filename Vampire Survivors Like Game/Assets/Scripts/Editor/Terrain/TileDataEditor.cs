using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


#if (UNITY_EDITOR) 
[CustomEditor(typeof(TerrainData))]
public class TerrainDataEditor : Editor
{
    public VisualTreeAsset m_InspectorXML;

    
    public override VisualElement CreateInspectorGUI()
    {
    
        // Create a new VisualElement to be the root of our Inspector UI.
        VisualElement myInspector = new VisualElement();

        // Load the UXML file.
        m_InspectorXML= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/Terrain/TerrainDataUXML.uxml");

        // Instantiate the UXML.
        myInspector = m_InspectorXML.Instantiate();

        // Return the finished Inspector UI.
        return myInspector;


    }

    void OnValidate(){
        SaveScriptable();
    }

    void SaveScriptable(){
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif