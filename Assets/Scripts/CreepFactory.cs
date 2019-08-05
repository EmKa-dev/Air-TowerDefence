using System;
using System.Linq;
using UnityEngine;

public class CreepFactory
{
    private GameObject[] _CreepPrefabs;

    public CreepFactory()
    {
        LoadCreepResources();

        if (CheckAllAreUnique())
        {
            throw new Exception("Duplicate names found in CreepPrefabs, please make sure they are all unique");
        }
    }

    private bool CheckAllAreUnique()
    {
        return _CreepPrefabs.Distinct().Count() == _CreepPrefabs.Count() ? false : true;
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
