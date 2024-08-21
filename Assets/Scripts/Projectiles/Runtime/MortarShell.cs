using System;
using Data;
using TrainScripts;
using Units;
using UnityEngine;

namespace Projectiles
{
    public class MortarShell : MonoBehaviour
    {
        [SerializeField] protected new Rigidbody rigidbody;
        [SerializeField] protected new SphereCollider collider;
        [SerializeField] protected GameObject meshObject;
        [SerializeField] protected ParticleSystem explosionEffectParticles;
        [SerializeField] protected float explosionImpactRadius = 3f;
        [SerializeField] protected LayerMask enemyLayerMask;

        private Transform m_CachedTransform;
        private Quaternion m_InitialRotation;

        public LayerMask EnemyLayerMask { get => enemyLayerMask; set => enemyLayerMask = value; }
        public float ExplosionImpactRadius
        {
            get => explosionImpactRadius;
            set => explosionImpactRadius = value;
        }
        public Vector3 InitialForce
        {
            set => rigidbody.velocity = value;
        }
        
        public float ColliderRadius => collider.radius;

        public UnitCombatStatCaptureData DamageContextData { get; set; }
        
        public Transform CachedTransform => m_CachedTransform ? m_CachedTransform : (m_CachedTransform = GetComponent<Transform>());

        private void Awake()
        {
            if (explosionEffectParticles == null)
            {
                explosionEffectParticles = null;
            }
        }

        private void Update()
        {
            if (!isActiveAndEnabled || !collider.enabled)
            {
                return;
            }

            CachedTransform.rotation = Quaternion.LookRotation(rigidbody.velocity);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (((1 << other.gameObject.layer) & enemyLayerMask) == 0)
            {
                return;
            }
            rigidbody.isKinematic = true; // because `Rigidbody` does not have `enabled` property, switch to kinematic mode to stop the shell from moving
            collider.enabled = false;
            meshObject.SetActive(false);
            
            explosionEffectParticles?.Play();

            var hitColliders = new Collider[8];
            var hitCount = Physics.OverlapSphereNonAlloc(CachedTransform.position, explosionImpactRadius,
                hitColliders, enemyLayerMask.value, QueryTriggerInteraction.Ignore);

            for (var i = 0; i < hitCount; ++i)
            {
                if (hitColliders[i].TryGetComponent(out ICombatEventReceiver eventReceiver))
                {
                    eventReceiver.TakeDamage(DamageContextData);
                }
            }
            
            Destroy(gameObject, 3f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(CachedTransform.position, explosionImpactRadius);
        }
    }
}