using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using System;

namespace Tests
{
    public class MultiTargetTests
    {

        // A Test behaves as an ordinary method
        [Test]
        public void NoTargetsInScene_ShouldDetectNoTargets()
        {
            var t = SetupBehaviourAndCleanScene();

            t.UpdateTargets();

            Assert.True(t.Targets.Where(x => x != null).Count() == 0);
        }

        [Test]
        public void OneTargetOutOfRange_ShouldNotDetectTarget()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            MoveGameObjectOutOfRange(creep);

            t.UpdateTargets();

            Assert.True(t.Targets.All(x => x == null));
        }

        [Test]
        public void OneNearbyTarget_ShouldDetectOneTarget()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            t.UpdateTargets();

            Assert.True(t.Targets.Where(x => x != null).Count() == 1);
        }

        [Test]
        public void OneNearbyTarget_OneTargetMoveOutOfRange_ShouldDetectNoTargets()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            t.UpdateTargets();

            if (t.Targets.Where(x => x != null).Count() == 1)
            {
                MoveGameObjectOutOfRange(creep);

                t.UpdateTargets();

                Assert.True(t.Targets.All(x => x == null));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void ThreeNearbyTarget_ShouldDetectThreeTargets()
        {
            var t = SetupBehaviourAndCleanScene();

            var creeps = new[]
            {
                SetupCreep(),
                SetupCreep(),
                SetupCreep()
            };

            t.UpdateTargets();

            Assert.True(t.Targets.Where(x => x != null).Count() == 3);
        }

        [Test]
        public void ThreeNearbyTarget_OneMoveOutOfRange_ShouldHaveTwoTargetsLeft_OneSlotNull()
        {
            var t = SetupBehaviourAndCleanScene();

            var creeps = new[]
            {
                SetupCreep(),
                SetupCreep(),
                SetupCreep()
            };

            t.UpdateTargets();

            if (t.Targets.Where(x => x != null).Count() == 3)
            {
                MoveGameObjectOutOfRange(creeps.First());

                t.UpdateTargets();

                Assert.True(t.Targets.Where(x => x != null).Count() == 2 && t.Targets.Where(z => z == null).Count() == 1);
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void ShouldSkipAlreadyTargettedTarget()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            //Detect one creep
            t.UpdateTargets();

            if (t.Targets.Any(x => object.ReferenceEquals(creep.transform, x)))
            {
                t.UpdateTargets();

                bool noduplicate = t.Targets.Where(z => object.ReferenceEquals(creep.transform, z)).Count() == 1;

                Assert.True(noduplicate);
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void ShouldHoldOnToStillAliveAndWithinRangeTarget()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            t.UpdateTargets();

            var firsttargetindex = Array.IndexOf(t.Targets, creep.transform);

            if (t.Targets.Where(x => x != null).Count() == 1)
            {
                //Add three more creeps
                SetupCreep();
                SetupCreep();
                SetupCreep();

                //Move first creep slightly so it is no longer the closest
                creep.transform.position = Vector3.one;

                //Now 4 is withinrange
                t.UpdateTargets();

                if (t.Targets.Where(x => x != null).Count() == 3)
                {
                    //Assert first creep is still in the slot it was in previously
                    Assert.True(object.ReferenceEquals(creep.transform, t.Targets[firsttargetindex]));
                }
                else
                {
                    Assert.Inconclusive();
                }
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void ShouldChooseClosestTargets()
        {
            var t = SetupBehaviourAndCleanScene();

            List<GameObject> creeps = new List<GameObject>();

            for (int i = 0; i < 10; i++)
            {
                var cri = SetupCreep();
                cri.transform.position = new Vector3(i + 0.5f, 0, 0);
                creeps.Add(cri);
            }

            t.UpdateTargets();

            if (t.Targets.Where(x => x != null).Count() == t.Targets.Length)
            {
                bool targetedbydistance = true;

                for (int i = 0; i < t.Targets.Length; i++)
                {
                    if (t.Targets[i].transform.position != creeps[i].transform.position)
                    {
                        targetedbydistance = false;
                    }
                }

                Assert.True(targetedbydistance);
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        private MultiTargetter SetupBehaviourAndCleanScene()
        {
            CleanupScene();

            MultiTargetter mt = new GameObject("TestMultiTargetter").AddComponent<MultiTargetter>();
            mt.Targets = new Transform[3];
            mt.Range = 10f;

            return mt;
        }

        private GameObject SetupCreep()
        {
            var creep = new GameObject($"TestCreep{GameObject.FindObjectsOfType<GameObject>().Where(x => x.layer == 10).Count() + 1}");
            creep.layer = 10;
            creep.AddComponent<BoxCollider>();

            return creep;
        }

        private void MoveGameObjectOutOfRange(GameObject gameobject)
        {
            gameobject.transform.position = Vector3.one * 5000;

            //Apparently colliders positions doesn't get updated along with its gameobject
            //in test playmode, so we need to move it manually

            gameobject.GetComponent<BoxCollider>().center = Vector3.zero;

        }

        private void CleanupScene()
        {
            foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
            {
                GameObject.DestroyImmediate(obj);
            }
        }
    }
}
