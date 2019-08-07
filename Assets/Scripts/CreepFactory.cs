using System;
using System.Linq;
using UnityEngine;

public class CreepFactory
{
    private GameObject[] _CreepPrefabs;

    public CreepFactory()
    {
        LoadCreepResources();
    }

    private void LoadCreepResources()
    {
        _CreepPrefabs = Resources.LoadAll("CreepPrefabs").Cast<GameObject>().ToArray();
    }

    public GameObject GetCreepPrefab(string creeptype)
    {
        return _CreepPrefabs.Single(x => x.name == creeptype);
    }
}
