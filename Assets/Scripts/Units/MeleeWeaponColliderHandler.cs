using TrainScripts;
using UnityEngine;

public class MeleeWeaponColliderHandler : MonoBehaviour
{
    public event OnWeaponCollidedDelegate OnWeaponCollided;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Car car))
        {
            OnWeaponCollided?.Invoke(car.gameObject);
        }
    }

    public delegate void OnWeaponCollidedDelegate(GameObject other);
}
