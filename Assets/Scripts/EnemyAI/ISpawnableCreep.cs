using AirTowerDefence.EnemySpawn;

namespace AirTowerDefence.Enemy
{

    public interface ISpawnableCreep
    {
        void Initialize(Waypoint spawnpoint);
    }
}

