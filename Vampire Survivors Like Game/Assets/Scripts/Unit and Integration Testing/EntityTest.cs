using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

[TestFixture]
public class EntityTest
{

    GameObject exampleMob = Resources.Load("Prefabs/Entities/Mob/swordsman_mob") as GameObject;

    GameObject mobEntityInstance;

    GameObject gameController;

    bool oneTimeSetup = false;

    [SetUp]
    public void SetUp(){
        if (oneTimeSetup == false){
            gameController = new GameObject("g");
            gameController.AddComponent<GameController>().RestartGameState();
            oneTimeSetup = true;
        }

        mobEntityInstance = GameObject.Instantiate(exampleMob, new Vector3(80, 80, 0), Quaternion.Euler(0,0,0));
        mobEntityInstance.GetComponent<MobEntity>().SetPrefabName("swordsman_mob");
    }

    [TearDown]
    public void TearDown(){
        GameObject.DestroyImmediate(mobEntityInstance);
        gameController.GetComponent<GameController>().RestartGameState();
    }

    /// <summary>
    /// Checks for following conditions upon mob initialization:
    /// 1. if mob object actually gets loaded on initialization
    /// 2. proper components have been initialized
    /// </summary>
    [Test]
    public void EmptyEntityTestInstantiation()
    {
        Assert.NotNull(mobEntityInstance);
        Assert.NotNull(mobEntityInstance.GetComponent<MobEntity>());
        Assert.NotNull(mobEntityInstance.GetComponent<CharacterMovementController>());
        Assert.NotNull(mobEntityInstance.GetComponent<WeaponController>());
    }

    /// <summary>
    /// Checks if entity can take damage, and that damage taken is the proper value
    /// </summary>
    [Test]
    public void EntityTestDamageTaken()
    {
        int damage = 5;
        int startingHealth = mobEntityInstance.GetComponent<MobEntity>().GetHealth();

        mobEntityInstance.GetComponent<MobEntity>().TakeDamage(damage);
        int damagedHealth = mobEntityInstance.GetComponent<MobEntity>().GetHealth();

        Assert.AreEqual(startingHealth - damage, damagedHealth);
    }

    /// <summary>
    /// Test if entity will move into range of enemy in order to fire
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator EntityTestFiringOutsideRange()
    {
        float deltaTime = 0;

        while (GameObject.FindObjectOfType<ProjectileEntity>() == null && deltaTime < 5){
            yield return new WaitForSeconds(0.1f);
            deltaTime += 0.1f;
        }

        Assert.That(GameObject.FindObjectOfType<ProjectileEntity>(), !Is.Null);
    }

    /// <summary>
    /// Base case testing of if the entity will fire projectile at enemy at all
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator EntityTestFiringInsideRange()
    {
        mobEntityInstance.transform.position =  new Vector3(20, 20, 0);

        float deltaTime = 0;

        while (GameObject.FindObjectOfType<ProjectileEntity>() == null && deltaTime < 5){
            yield return new WaitForSeconds(0.1f);
            deltaTime += 0.1f;
        }

        Assert.That(GameObject.FindObjectOfType<ProjectileEntity>(), !Is.Null);
    }
}
