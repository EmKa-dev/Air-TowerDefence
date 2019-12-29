using AirTowerDefence.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AirTowerDefence.Managers
{
    public class Shop : MonoBehaviour
    {
        #region Singleton

        private static Shop _Instance;

        public static Shop Instance
        {
            get
            {
                if (_Instance == null)
                {
                    // Search for existing instance.
                    _Instance = (Shop)FindObjectOfType(typeof(Shop));

                    // Create new instance if one doesn't already exist.
                    if (_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        _Instance = singletonObject.AddComponent<Shop>();
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

        [SerializeField]
        private RectTransform _ShopIconPrefab;

        [SerializeField]
        private List<ShopItem> _ShopItems;

        [SerializeField]
        private RectTransform _TowerShopPanel;

        public List<ShopItem> ShopItems
        {
            get { return _ShopItems; }
        }

        void Awake()
        {

            _ShopItems.Sort((x, y) => x.Cost.CompareTo(y.Cost));

            foreach (var item in _ShopItems)
            {
                var element = Instantiate(_ShopIconPrefab, _TowerShopPanel);

                element.GetComponent<Image>().sprite = item.Icon;

                element.GetComponentInChildren<Text>().text = item.Cost.ToString();
            }
        }
    }
}
