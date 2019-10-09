using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AirTowerDefence.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _Instance;

        public static GameManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GameManager();
                }
                return _Instance;
            }
            private set { _Instance = value; }
        }

        public Transform Player { get; set; }


        void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
