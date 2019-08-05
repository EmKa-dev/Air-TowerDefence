using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WavesContainer
{
    public List<Wave> Waves = new List<Wave>();

    public void AddWave(Wave wavedata)
    {
        Waves.Add(wavedata);
    }
}
