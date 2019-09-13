using UnityEngine;

public interface ILauncher
{
    void Fire(Transform target);

    void Fire(Transform[] targets);
}