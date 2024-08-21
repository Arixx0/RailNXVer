using Data;
using Units;
using UnityEngine;

namespace Projectiles
{
    [DisallowMultipleComponent]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected float collisionRadius;
        [SerializeField] protected float speed;
        [SerializeField] protected float maxTravelDistance;
        [SerializeField] protected LayerMask hitMask;
        protected UnitCombatStatCaptureData combatStatCaptureData;

        private Transform m_CachedTransform;
        
        private readonly Collider[] m_HitColliderCache = new Collider[2];

        public float CurrentTravelDistance { get; private set; }

        public string GameObjectName
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public Vector3 TransformForward
        {
            get => CachedTransform.forward;
            set => CachedTransform.forward = value;
        }

        public UnitCombatStatCaptureData CombatStatData
        {
            set => combatStatCaptureData = value;
        }
        
        public Transform CachedTransform => m_CachedTransform ? m_CachedTransform : (m_CachedTransform = transform);

        public event OnHitEventDelegate OnHit;

        private void FixedUpdate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (CheckIfHit())
            {
                if (m_HitColliderCache[0].TryGetComponent(out ICombatEventReceiver eventReceiver))
                {
                    Debug.Log($"{m_HitColliderCache[0].gameObject.name} took damage {combatStatCaptureData.AttackDamage}", gameObject);
                    eventReceiver.TakeDamage(combatStatCaptureData);
                }
                
                // OnHit?.Invoke(m_HitColliderCache[0]);
                
                DoDestroy();
                return;
            }
            
            if (CurrentTravelDistance >= maxTravelDistance)
            {
                DoDestroy();
                return;
            }
            
            var velocity = speed * Time.deltaTime * TimeScaleManager.Get.GetTimeScaleOfTag(tag);
            CachedTransform.position += CachedTransform.forward * velocity;
            CurrentTravelDistance += velocity;
        }
        
        private void DoDestroy()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private bool CheckIfHit()
        {
            // NOTE:
            //  Collision events are only sent if one of the colliders also has non-kinematic rigidbody attached.
            //  Because projectiles are going to implements own physics process, check the collision manually.
            
            var hitCount = Physics.OverlapSphereNonAlloc(
                CachedTransform.position, collisionRadius, m_HitColliderCache, hitMask);
            var didHitSomething = hitCount > 0;
            return didHitSomething;
        }

        public void SetProjectileInfo(float speed, int hitMask)
        {
            this.speed = speed;
            this.hitMask = hitMask;
            maxTravelDistance = speed * speed;
        }

        public delegate void OnHitEventDelegate(Collider hitCollider);
    }
}