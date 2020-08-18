using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScript : MonoBehaviour
{

    public GameObject ObjectToDestroy;

    public void OnDeathAnimationEnded()
    {
        Destroy(ObjectToDestroy);
    }
}
