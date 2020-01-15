using AirTowerDefence.Managers;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    public abstract class AttackController : Controller
    {
        [SerializeField]
        protected float _AttackRate;

        [SerializeField]
        protected int _Damage;

        [SerializeField]
        protected float _ProjectileSpeed;
    }
}

