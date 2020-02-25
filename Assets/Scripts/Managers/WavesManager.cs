using AirTowerDefence.Enemy;
using AirTowerDefence.EnemySpawn;
using System.Collections;
using UnityEngine;

namespace AirTowerDefence.Managers
{
    public class WavesManager : MonoBehaviour
    {
        public WavesContainer WavesContainer;

        private CreepFactory _CreepFactory;
        private Waypoint _Spawnpoint;

        public int WaveIndex { get; private set; } = 0;

        private void Start()
        {
            _CreepFactory = new CreepFactory();
            _Spawnpoint = GetComponent<WaypointManager>().GetSpawnPoint();

            if (WavesContainer.Waves.Count > 0)
            {
                StartNextWave();
            }
        }

        public void StartNextWave()
        {
            StartCoroutine(StartWave(WavesContainer.Waves[WaveIndex]));
            WaveIndex++;
        }

        IEnumerator StartWave(Wave wave)
        {
            yield return new WaitForSecondsRealtime(1f);

            foreach (var spawn in wave.Spawns)
            {
                SpawnCreeps(spawn);

                yield return new WaitForSecondsRealtime(spawn.TimeToNext);
            }
        }


        private void SpawnCreeps(Spawn spawn)
        {
            foreach (var datapoint in spawn.SpawnData)
            {
                var creep = Instantiate(_CreepFactory.GetCreepPrefab(datapoint.CreepIdentifier));

                creep.transform.position = _Spawnpoint.transform.TransformPoint(datapoint.RelativePosition);
                ISpawnableCreep c = creep.GetComponentInChildren<ISpawnableCreep>();
                c.Initialize(_Spawnpoint);
            }
        }
    }
}
