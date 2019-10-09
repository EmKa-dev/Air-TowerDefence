using System.Collections.Generic;
using System.Linq;
using AirTowerDefence.Tower;
using NUnit.Framework;
using UnityEngine;

namespace AirTowerDefence.Tests
{
    public class SingleTargetTests
    {
        [Test]
        public void NoTargetsInScene_ShouldDetectNoTargets()
        {
            var t = SetupBehaviourAndCleanScene();

            t.Search();

            Assert.True(t.Target == null);
        }

        [Test]
        public void OneTargetOutOfRange_ShouldNotDetectTarget()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            MoveGameObjectOutOfRange(creep);

            t.Search();

            Assert.True(t.Target == null);
        }

        [Test]
        public void OneNearbyTarget_ShouldDetectOneTarget()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            t.Search();

            Assert.True(t.Target != null);
        }

        [Test]
        public void OneNearbyTarget_OneTargetMoveOutOfRange_ShouldDetectNoTargets()
        {
            var t = SetupBehaviourAndCleanScene();
            var creep = SetupCreep();

            t.Search();

            if (t.Target != null)
            {
                MoveGameObjectOutOfRange(creep);

                t.Search();

                Assert.True(t.Target == null);
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

            t.Search();

            if (t.Target != null)
            {
                //Add three more creeps
                SetupCreep();
                SetupCreep();
                SetupCreep();

                //Move first creep slightly so it is no longer the closest
                creep.transform.position = Vector3.one;

                //Now 4 is withinrange
                t.Search();

                if (t.Target != null)
                {
                    //Assert first creep is still the target
                    Assert.True(object.ReferenceEquals(creep.transform, t.Target));
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
        public void ShouldChooseClosestTarget()
        {
            var t = SetupBehaviourAndCleanScene();

            List<GameObject> creeps = new List<GameObject>();

            for (int i = 0; i < 10; i++)
            {
                var cri = SetupCreep();
                cri.transform.position = new Vector3(i + 0.5f, 0, 0);
                creeps.Add(cri);
            }

            t.Search();

            if (t.Target != null)
            {
                Assert.True(object.ReferenceEquals(creeps.First().transform, t.Target));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        private SingleTargetter SetupBehaviourAndCleanScene()
        {
            CleanupScene();

            SingleTargetter st = new GameObject("TestMultiTargetter").AddComponent<SingleTargetter>();
            st.Range = 10f;

            return st;
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
