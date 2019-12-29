using UnityEngine;

// http://wiki.unity3d.com/index.php?title=Singleton&_ga=2.88899931.1186458435.1570607649-2048233608.1562962032

namespace AirTowerDefence.Managers
{
    public class GameManager : MonoBehaviour
    {

        #region Singleton
        private static GameManager _Instance;

        public static GameManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    // Search for existing instance.
                    _Instance = (GameManager)FindObjectOfType(typeof(GameManager));

                    // Create new instance if one doesn't already exist.
                    if (_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        _Instance = singletonObject.AddComponent<GameManager>();
                        singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";

                        // Make instance persistent.
                        //DontDestroyOnLoad(singletonObject);
                    }
                }
                return _Instance;
            }
            private set { _Instance = value; }
        }

        #endregion

        public Transform Player { get; set; }

        void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
