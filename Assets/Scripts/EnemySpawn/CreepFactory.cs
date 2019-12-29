using System.Linq;
using UnityEngine;

//TODO:
//Make this class shared or static/singelton (no point in multiple instances of this)
namespace AirTowerDefence.EnemySpawn
{
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
}
