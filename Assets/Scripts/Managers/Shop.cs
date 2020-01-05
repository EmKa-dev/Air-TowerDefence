using AirTowerDefence.Common;
using System;
using System.Collections;
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
        private RectTransform _SelectionMarkerPrefab;

        [SerializeField]
        private List<ShopItem> _ShopItems;

        [SerializeField]
        private RectTransform _TowerShopPanel;

        private RectTransform _SelectionMarker;

        private int _SelectionIndex = 0;

        private Transform[] _ShopPanelElements;

        public ShopItem SelectedItem
        {
            get => _ShopItems[_SelectionIndex];
        }

        void Awake()
        {
            StartCoroutine("Initialize");
        }

        private IEnumerator Initialize()
        {
            InitializeShopPanel();

            _ShopPanelElements = GetChildrenArray(_TowerShopPanel);

            yield return new WaitForEndOfFrame();

            InitializeSelectionMarker();
        }


        private void InitializeShopPanel()
        {
            _ShopItems.Sort((x, y) => x.Cost.CompareTo(y.Cost));

            foreach (var item in _ShopItems)
            {
                var element = Instantiate(_ShopIconPrefab, _TowerShopPanel);

                element.GetComponent<Image>().sprite = item.Icon;

                element.GetComponentInChildren<Text>().text = item.Cost.ToString();
            }
        }

        private Transform[] GetChildrenArray(RectTransform panel)
        {
            var arr = new Transform[panel.childCount];

            for (int i = 0; i < panel.childCount; i++)
            {
                arr[i] = panel.GetChild(i);
            }

            return arr;
        }

        private void InitializeSelectionMarker()
        {
            _SelectionMarker = Instantiate(_SelectionMarkerPrefab);

            _SelectionMarker.SetParent(_TowerShopPanel);

            var target = _ShopPanelElements[_SelectionIndex];

            _SelectionMarker.localPosition = new Vector3(target.localPosition.x, target.localPosition.y);
            _SelectionMarker.localScale = Vector3.one;
        }

        public void SwitchSelectedItem()
        {
            _SelectionIndex++;

            if (_SelectionIndex >= _ShopPanelElements.Length)
            {
                _SelectionIndex = 0;
            }

            var target = _ShopPanelElements[_SelectionIndex];

            _SelectionMarker.localPosition = new Vector3(target.localPosition.x, target.localPosition.y);
        }
    }
}
