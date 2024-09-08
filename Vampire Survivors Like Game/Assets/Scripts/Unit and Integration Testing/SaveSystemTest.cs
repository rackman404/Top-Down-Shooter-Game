using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEditor;
using System.ComponentModel;

/// <summary>
/// Unit testing for the save-loading system.
/// </summary>
[TestFixture]
public class SaveSystemTest
{

    ISaveLoadHandler handlerInstance;

    LevelData lvlDataInstance;

    bool oneTimeSetup = false;

    [SetUp]
    public void SetUp(){
        if (oneTimeSetup == false){
            handlerInstance = new GameObject().AddComponent<SaveLoadHandler>();
            oneTimeSetup = true;
        }
        
        lvlDataInstance = new LevelData();
    }

    [TearDown]
    public void TearDown(){
        lvlDataInstance = null;
    }

    /// <summary>
    /// 
    /// </summary>
    [Test]
    public void SaveLoadEmptyData()
    {
        
    }

    [Test]
    public void SaveLoadTestData()
    {

    }

}
