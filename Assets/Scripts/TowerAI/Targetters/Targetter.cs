using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Targetter : MonoBehaviour
{
    [Header("Attributes")]

    public float Range;

    public float SearchRate;

    [Header("Setup")]

    [SerializeField]
    protected GameObject _TurretMesh;

    protected Collider[] _CandiateBuffer;

    protected int _TargetLayerMask;

    public abstract void Search();
}

