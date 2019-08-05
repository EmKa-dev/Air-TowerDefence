using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Vector3 Position { get; private set; }

    [SerializeField]
    public Waypoint Next;

    void Start()
    {
        Position = transform.position;
    }
}
