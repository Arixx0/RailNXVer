using Units;
using Units.Stats;
using UnityEngine;

namespace Projectiles
{
    public enum ProjectileType
    {
        BasicRifle,
        ShotGun,
        FlameThrower,
        AssaultRifle
    }
    public class ProjectileParticleHandler : MonoBehaviour
    {
        #region Projectile Particle Handler Components

        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private ProjectileType projectileType;
        [SerializeField] private short minProjectileCount;
        [SerializeField] private short maxProjectileCount;
        [SerializeField] private float projectileRange;

        private ParticleSystem.Burst[] m_ParticleBurstInfo;
        private BurstParticleInfo[] m_BurstParticleInfo;
        private float m_ParticleEmissionRate = -1;
        private float m_ParticleEmissionCooldown = -1;
        private float m_LastParticleTime = -1;
        private bool m_didParticleEmit;

        public ProjectileType ProjectileType
        {
            get => projectileType;
            set => projectileType = value;
        }

        public short MinProjectileCount
        {
            get => minProjectileCount;
            set => minProjectileCount = value;
        }

        public short MaxProjectileCount
        {
            get => maxProjectileCount;
            set => maxProjectileCount = value;
        }

        public float ProjectileRange
        {
            get => projectileRange;
            set => projectileRange = value;
        }

        #endregion

        public event OnParticleEmittedDelegate OnParticleEmitted;

        public event OnParticleCollidedDelegate OnParticleCollided;

        #region MonoBehaviour Events

        private void Awake()
        {
            CacheParticleEmissionInformation();
        }

        private void Update()
        {
            if (!particleSystem.isEmitting)
            {
                return;
            }
            
            var time = particleSystem.time;

            m_didParticleEmit = false;
            
            // Emit particle based on emission rate
            m_ParticleEmissionCooldown += Time.deltaTime;
            if (m_ParticleEmissionCooldown >= m_ParticleEmissionRate)
            {
                m_ParticleEmissionCooldown -= m_ParticleEmissionRate;
                m_didParticleEmit = true;
            }
            // Emit particle based on burst settings
            var resetBurstInfo = m_LastParticleTime < 0 || time < m_LastParticleTime;
            for (var i = 0; i < m_BurstParticleInfo.Length; ++i)
            {
                m_didParticleEmit |= m_BurstParticleInfo[i].Emit(resetBurstInfo, time);
            }
            
            m_LastParticleTime = time;

            if (m_didParticleEmit)
            {
                OnParticleEmitted?.Invoke();
            }
        }

        #endregion

        public void SetParticleCollisionLayer(LayerMask collisionLayer)
        {
            var collisionModule = particleSystem.collision;
            collisionModule.collidesWith = collisionLayer;
        }

        /// <summary>Adjust ParticleSystem.EmissionModule.rateOverTime</summary>
        /// <param name="emissionRate">the rate at which the emitter spawns new particles over time</param>
        public void SetParticleEmissionRate(float emissionRate)
        {
            var emissionModule = particleSystem.emission;
            emissionModule.rateOverTime = emissionRate;
            m_ParticleEmissionRate = 1 / emissionRate;
        }

        public void SetProjectileType(UnitStatComponent statComponent, float fireInterval = 0.1f)
        {
            var mainModule = particleSystem.main;
            var emissionModule = particleSystem.emission;
            var shapeModule = particleSystem.shape;
            m_ParticleBurstInfo = new ParticleSystem.Burst[10];
            var basicBurst = new ParticleSystem.Burst
            {
                time = 0f,
                count = 1f,
                cycleCount = 1,
                repeatInterval = 0.1f,
                probability = 1f
            };
            switch (projectileType)
            {
                case ProjectileType.BasicRifle:
                    m_ParticleBurstInfo = new ParticleSystem.Burst[] { basicBurst };
                    particleSystem.emission.SetBursts(m_ParticleBurstInfo);
                    SetParticleDuration(float.PositiveInfinity);
                    break;
                case ProjectileType.ShotGun:
                    var shotgunBurst = new ParticleSystem.Burst
                    {
                        time = 0f,
                        count = new ParticleSystem.MinMaxCurve(minProjectileCount, maxProjectileCount),
                        cycleCount = 0,
                        repeatInterval = 1 / statComponent.AttackSpeed,
                        probability = 1f
                    };
                    m_ParticleBurstInfo = new ParticleSystem.Burst[] { shotgunBurst };
                    particleSystem.emission.SetBursts(m_ParticleBurstInfo);
                    SetParticleDuration(1 / statComponent.AttackSpeed);
                    shapeModule.angle = projectileRange;
                    break;
                case ProjectileType.FlameThrower:
                    emissionModule.rateOverTime = statComponent.AttackSpeed * 60f;
                    mainModule.startSpeed = statComponent.AttackSpeed;
                    mainModule.startLifetime = statComponent.AttackSpeed * statComponent.AttackSpeed;
                    shapeModule.angle = projectileRange;
                    break;
                case ProjectileType.AssaultRifle:
                    m_ParticleBurstInfo = new ParticleSystem.Burst[3];
                    m_ParticleBurstInfo[0] = basicBurst;
                    for (int i = 1; i <= 2; i++)
                    {
                        m_ParticleBurstInfo[i] = new ParticleSystem.Burst
                        {
                            time = fireInterval * i,
                            count = 1f,
                            cycleCount = 0,
                            repeatInterval = 1 / statComponent.AttackSpeed,
                            probability = 1f
                        };
                    }
                    particleSystem.emission.SetBursts(m_ParticleBurstInfo);
                    SetParticleDuration(float.PositiveInfinity);
                    break;
                default:
                    break;
            }
        }

        public void StartEmitting()
        {
            if (particleSystem.isEmitting)
            {
                return;
            }
            
            particleSystem.Play();
            m_ParticleEmissionCooldown = m_ParticleEmissionRate;
            m_LastParticleTime = -1;
        }

        public void PauseEmitting()
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        private void OnParticleCollision(GameObject other)
        {
            OnParticleCollided?.Invoke(other);
        }

        private void CacheParticleEmissionInformation()
        {
            var emissionModule = particleSystem.emission;
            
            m_ParticleEmissionRate = 1 / emissionModule.rateOverTime.constant;
            
            var burstCount = emissionModule.burstCount;
            
            m_ParticleBurstInfo = new ParticleSystem.Burst[burstCount];
            particleSystem.emission.GetBursts(m_ParticleBurstInfo);

            m_BurstParticleInfo = new BurstParticleInfo[burstCount];
            for (var i = 0; i < burstCount; ++i)
            {
                m_BurstParticleInfo[i] = new BurstParticleInfo(m_ParticleBurstInfo[i]);
            }
        }

        private void SetParticleDuration(float duration)
        {
            var mainModule = particleSystem.main;
            mainModule.duration = duration;
        }

        public delegate void OnParticleEmittedDelegate();

        public delegate void OnParticleCollidedDelegate(GameObject other);

        private struct BurstParticleInfo
        {
            private readonly float m_CycleCount;
            private readonly float m_RepeatInterval;
            private readonly float m_Time;

            private float m_LastEmissionTime;
            private float m_EmissionCount;
            
            public BurstParticleInfo(ParticleSystem.Burst burst)
            {
                m_CycleCount = burst.cycleCount;
                m_RepeatInterval = burst.repeatInterval;
                m_Time = burst.time;

                m_LastEmissionTime = -1;
                m_EmissionCount = m_CycleCount;
            }
            
            /// <summary>Emit burst based on settings property.</summary>
            /// <param name="reset">Reset internal state properties.</param>
            /// <param name="particleTime">Elapsed time of parent particle system.</param>
            /// <returns>TRUE if burst emitted</returns>
            public bool Emit(bool reset, float particleTime)
            {
                if (reset)
                {
                    m_LastEmissionTime = -1;
                    m_EmissionCount = m_CycleCount;
                }

                if (m_LastEmissionTime >= particleTime ||
                    m_EmissionCount <= 0 ||
                    (m_LastEmissionTime >= 0 && particleTime < m_LastEmissionTime + m_RepeatInterval))
                {
                    return false;
                }

                m_EmissionCount -= 1;
                m_LastEmissionTime = m_Time;
                return true;
            }
        }
    }
}