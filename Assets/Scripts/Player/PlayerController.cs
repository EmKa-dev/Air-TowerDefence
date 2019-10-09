using UnityEngine;

namespace AirTowerDefence.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float Speed;
        [SerializeField]
        private float Sensitivity;

        private Vector2 mouseinp;

        void Update()
        {
            // Translation
            var translation = GetInputTranslationDirection() * Time.deltaTime;

            mouseinp = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            transform.Rotate(Vector3.up * mouseinp.x * Sensitivity);

            transform.Translate(translation * Speed);
        }

        private void GetInputRotation()
        {


        }

        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }

            return direction;
        }
    }
}
