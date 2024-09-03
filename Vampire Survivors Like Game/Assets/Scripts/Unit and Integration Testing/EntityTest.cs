using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class EntityTest
{

    GameObject exampleMob = Resources.Load("Prefabs/Entities/Mob/swordsman_mob") as GameObject;

    GameObject mobEntityInstance;

    bool alreadyInited = false;

    [SetUp]
    public void SetUp(){
        GameObject mobEntityInstance = GameObject.Instantiate(exampleMob);
    }

    /// <summary>
    /// Checks for following conditions upon mob initialization:
    /// 1. if mob actually gets loaded (is not null)
    /// 2. proper components have been initialized
    /// </summary>
    [UnityTest]
    public IEnumerator EmptyEntityTestInstantiation()
    {
        Assert.That(mobEntityInstance.GetComponent<MobEntity>(), !Is.Null);

        yield return null;

        Assert.That(mobEntityInstance.GetComponent<CharacterMovementController>(), !Is.Null);
        Assert.That(mobEntityInstance, !Is.Null);
    }

    /// <summary>
    /// Checks if entity can take damage, and that damage taken is the proper value
    /// </summary>
    [Test]
    public void EntityTestDamageTaken()
    {
        int damage = 5;

        int startingHealth = mobEntityInstance.GetComponent<MobEntity>().GetHealth();

        int damagedHealth = mobEntityInstance.GetComponent<MobEntity>().GetHealth();

        Assert.AreEqual(startingHealth - damage, damagedHealth);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator EntityTestFiring()
    {
        GameObject playerInstance = GameObject.Instantiate(Resources.Load("Prefabs/Entities/player") as GameObject, null);

        mobEntityInstance.transform.position = new Vector3(25, 25, 0);

        yield return null;
        yield return null;
        yield return null;

        Assert.That(GameObject.FindObjectOfType<ProjectileEntity>(), !Is.Null);
    }
}
