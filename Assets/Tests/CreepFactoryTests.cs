using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CreepFactoryTests
    {

        [Test]
        public void ShouldThrowIfDuplicateNamesInCreepPrefabsResources()
        {
            //Create dummy duplicates in Resources/CreepPrefabs


            Assert.Throws<Exception>(() => { new CreepFactory(); });
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CreepFactTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
