using TrainScripts;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileHarpoon : MonoBehaviour
    {
        private bool m_isConnected;

        public event OnHarpoonCollidedDelegate OnHarpoonCollided;
        public event OnHarpoonCollidedExitDelegate OnHarpoonCollidedExit;
        public bool isConnected
        {
            get => m_isConnected;
            set => m_isConnected = value;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Car car))
            {
                m_isConnected = true;
                OnHarpoonCollided?.Invoke(car.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Car car))
            {
                m_isConnected = false;
                OnHarpoonCollidedExit?.Invoke(car.gameObject);
            }
        }

        public delegate void OnHarpoonCollidedDelegate(GameObject other);
        public delegate void OnHarpoonCollidedExitDelegate(GameObject other);
    }
}