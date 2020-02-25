using UnityEngine;

namespace AirTowerDefence.Tower
{
    public abstract class Targetter : MonoBehaviour
    {
        [Header("Attributes")]

        public float Range;

        public float SecondsBetweenSearches;

        [Header("Setup")]

        [SerializeField]
        protected GameObject _TurretMesh;

        protected Collider[] _CandiateBuffer;

        protected int _TargetLayerMask;

        public abstract void Search();
    }
}

