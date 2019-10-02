using System;
using UnityEngine;

public interface ILauncher
{
    event Action Launch;

    void Fire(Transform target);

    void Fire(Transform[] targets);
}