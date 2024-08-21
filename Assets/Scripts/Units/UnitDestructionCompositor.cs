using UI;
using UnityEngine;

using Data;
using Utility;

namespace Units
{
    public class UnitDestructionCompositor : MonoBehaviour
    {
        [SerializeField] private GameObject rootMeshGameObject;
        [SerializeField] private ParticleSystem destructionVFXParticles;
        [SerializeField] private AudioSource destructionSFXSource;
        [SerializeField] private SFXPreset destructionSFXPreset;

        public bool IsDestroyed { get; private set; } = false;

        private void Awake()
        {
            if (destructionVFXParticles == null)
            {
                destructionVFXParticles = null;
            }
            else
            {
                destructionVFXParticles.gameObject.SetActive(false);
            }

            if (destructionSFXSource == null)
            {
                destructionSFXSource = null;
            }
            else
            {
                destructionSFXSource.gameObject.SetActive(false);
            }
        }

        public void DoDestroy(bool destroySelf = true, float destructionDelay = 3f)
        {
            if (IsDestroyed)
            {
                return;
            }

            IsDestroyed = true;
            
            UnitHealthBar healthBar = GetComponentInChildren<UnitHealthBar>();
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false);
            }

            if (TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }

            if (TryGetComponent(out Enemies.EnemyUnit enemyUnit) &&
                DatabaseUtility.TryGetData(Database.CircuitDropTable, enemyUnit.EnemyUnitIdentifier.Identifier, out var circuitDropTable))
            {
                // Using FindObjectofType as a stopgap measure
                // TODO : Access from singleton object having train
                TrainScripts.Train train = FindObjectOfType<TrainScripts.Train>();
                if (train != null && Random.value < circuitDropTable.possibility)
                {
                    Debug.Log($"Get Circuit {circuitDropTable.amount}");
                    train.AddResourceToInventory(ResourceType.Circuit, circuitDropTable.amount);
                }
            }
            
            rootMeshGameObject.SetActive(false);
            
            DoPlayDestructionEffects();

            if (destroySelf)
            {
                Destroy(gameObject, destructionDelay);
            }
        }

        public void DoPlayDestructionEffects()
        {
            if (destructionVFXParticles != null)
            {
                destructionVFXParticles.gameObject.SetActive(true);
                destructionVFXParticles.Play();
            }

            if (destructionSFXSource != null)
            {
                destructionSFXSource.gameObject.SetActive(true);
                if (destructionSFXPreset != null)
                {
                    destructionSFXSource.clip = destructionSFXPreset.GetRandomSFX();
                    destructionSFXSource.pitch = destructionSFXPreset.isRandomPitch ? destructionSFXPreset.GetRandomPitch() : 1f;
                }
                destructionSFXSource.Play();
            }
        }
    }
}