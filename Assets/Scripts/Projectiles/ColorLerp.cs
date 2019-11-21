using System;
using AirTowerDefence.Managers;
using UnityEngine;

namespace AirTowerDefence.Projectile
{
    public class ColorLerp : MonoBehaviour
    {
        private Renderer _Renderer;

        [SerializeField]
        private Color ColorA;

        [SerializeField]
        private Color ColorB;

        [SerializeField]
        private string ColorPropertyReferenceID;

        [SerializeField]
        private float StartLerpDistance;

        [SerializeField]
        private float MinLerpDistance;

        private Transform Target;


        private void Start()
        {
            _Renderer = GetComponent<Renderer>();
            _Renderer.material.SetColor(ColorPropertyReferenceID, ColorA);
            Target = GameManager.Instance.Player;
        }

        void Update()
        {
            float currentDistance = Vector3.Distance(this.transform.position, Target.position);

            if (currentDistance > StartLerpDistance)
            {
                return;
            }


            _Renderer.material.SetColor(ColorPropertyReferenceID, CalculateNewColor(currentDistance));
        }

        private Color CalculateNewColor(float currentDistance)
        {
            float percentage = Mathf.InverseLerp(StartLerpDistance, MinLerpDistance, currentDistance);

            return Color.Lerp(ColorA, ColorB, percentage);
        }
    }
}
