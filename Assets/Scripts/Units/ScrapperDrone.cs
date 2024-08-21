// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

using Attributes;
using Data;
using Environments;
using TrainScripts;
using UI;
using Utility;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Drones
{
    public class ScrapperDrone : MonoBehaviour, ICombatEventReceiver
    {
        [Header("Component References")]
        [SerializeField] protected UnitDestructionCompositor destructionCompositor;
        [SerializeField] protected BehaviorDesigner.Runtime.BehaviorTree behaviorTree;
        [SerializeField] protected Stats.ScrapperDroneStatComponent statComponent;
        [SerializeField] protected UnitHealthBar healthBar;
        [SerializeField] protected BoxCollider colliderComponent;
        [SerializeField] protected Animator animator;
        
        [Header("Scrapper Drone Properties")]
        [SerializeField] protected float timeForProduction;
        [SerializeField, Disabled] protected int resourceContainerTotalAmount;
        [SerializeField, Disabled] protected ResourceVein currentScrapTargetVein;
        
        [Header("SFX/VFX Compositions")]
        [SerializeField] protected AudioSource hoveringSFXAudioSource;
        [SerializeField] protected AudioSource drillingSFXAudioSource;
        [SerializeField] protected AudioClip[] drillingSFXAudioClips = Array.Empty<AudioClip>();
        [SerializeField] protected ParticleSystem drillingVFX;
        
        private Transform m_CachedTransform;
        private ScrapperCompartment m_ParentCompartment;
        protected Dictionary<ResourceType, int> m_ResourceContainer = new();
        private Vector3 m_CurrentMoveAmount;
        private Vector3 m_DisplacementToDestination;
        private float m_SqrDistanceToDestination;
        private float m_FacingFactorToDestination;
        private bool m_CanMoveTowardsTarget;
        private bool m_DoesArriveAtDestination;
        private System.Random m_LocalRandom = new();

        public Transform CachedTransform => (m_CachedTransform ??= GetComponent<Transform>());

        public ResourceVein CurrentScrapTargetVein
        {
            get => currentScrapTargetVein;
            set
            {
                currentScrapTargetVein = value;

                if (value)
                {
                    currentScrapTargetVein.onDestroyEvent += OnTargetedResourceVeinDestroyed;
                    
                    behaviorTree.SetVariableValue("scrapTargetGameObject", currentScrapTargetVein.gameObject);
                }
                else
                {
                    behaviorTree.SetVariableValue("scrapTargetGameObject", null);
                }
            }
        }

        public ScrapperCompartment ParentCompartment
        {
            get => m_ParentCompartment;
            set
            {
                m_ParentCompartment = value;
                behaviorTree.SetVariableValue("parentCompartmentGameObject", value == null ? null : value.gameObject);
            }
        }
        
        public float ForTimeForProduction => timeForProduction;
        
        public bool IsActing { get; private set; }
        
        public Units.Stats.ScrapperDroneStatComponent StatComponent => statComponent;

        protected virtual void Awake()
        {
            statComponent.Setup();

            healthBar = healthBar == null ? null : healthBar;
            if (healthBar != null)
            {
                healthBar.SetHealthProperties(statComponent.MaxHealth, statComponent.CurrentHealth);
                statComponent.OnHealthPointChanged += healthBar.UpdateHealthPoint;                
            }
        }
        
        public void DepositScrappedResource()
        {
            foreach (var pair in m_ResourceContainer)
            {
                ParentCompartment.DepositScrappedResource(pair.Key, pair.Value);
            }
            
            m_ResourceContainer.Clear();
            resourceContainerTotalAmount = 0;
        }

        public void ResetBehaviourState()
        {
            behaviorTree.EnableBehavior();
            IsActing = true;
        }

        private void ManipulateMovementProperties(Vector3 destination)
        {
            var position = CachedTransform.position;
            m_DisplacementToDestination = (destination - position).GetXZ();
            m_SqrDistanceToDestination = m_DisplacementToDestination.sqrMagnitude;
            m_FacingFactorToDestination = Vector3.Dot(CachedTransform.forward, m_DisplacementToDestination.normalized);
            m_DoesArriveAtDestination = m_SqrDistanceToDestination < statComponent.SqrAttackRange;
        }

        private void MoveTowards()
        {
            var deltaTime = Time.deltaTime;
            var moveAmount = Vector3.zero;

            m_CanMoveTowardsTarget = !m_DoesArriveAtDestination &&
                                     m_FacingFactorToDestination >= statComponent.TurnThresholdAngleAlpha;
            if (m_CanMoveTowardsTarget)
            {
                moveAmount += CachedTransform.forward.GetXZ().normalized * statComponent.MoveSpeed;
            }

            m_CurrentMoveAmount = Vector3.Lerp(m_CurrentMoveAmount, moveAmount, statComponent.MoveSpeedDamp);
            CachedTransform.position += m_CurrentMoveAmount * deltaTime;
        }

        private void RotateTowards()
        {
            var rotation = Quaternion.Lerp(CachedTransform.rotation,
                Quaternion.LookRotation(m_DisplacementToDestination, Vector3.up), statComponent.RotateSpeed);
            CachedTransform.rotation = rotation;
        }
        
        private bool TryScrapTargetVein()
        {
            if (CurrentScrapTargetVein == null)
            {
                return false;
            }
            
            // drillingSFXAudioSource.PlayOneShot(drillingSFXAudioClips[UnityEngine.Random.Range(0, drillingSFXAudioClips.Length)]);
            // drillingVFX.Play();

            var resourceType = CurrentScrapTargetVein.ResourceType;
            CurrentScrapTargetVein.TryTakeAwayResource((int)statComponent.ScrapAmountPerAction, out var miningAmount);

            if (!m_ResourceContainer.TryAdd(resourceType, miningAmount))
            {
                m_ResourceContainer[resourceType] += miningAmount;
            }
            resourceContainerTotalAmount += miningAmount;
            return true;
        }

        private void OnTargetedResourceVeinDestroyed()
        {
            ParentCompartment.TryGetResourceVein(out var newTarget);
            CurrentScrapTargetVein = newTarget;
            
            BehaviorDesigner.Runtime.BehaviorManager.instance.RestartBehavior(behaviorTree);
        }

        public void TakeDamage(UnitCombatStatCaptureData data)
        {
            statComponent.CurrentHealth -= (int)data.AttackDamage;

            if (statComponent.CurrentHealth <= 0)
            {
                colliderComponent.enabled = false;
                destructionCompositor.DoDestroy(false);

                StartCoroutine(NotifyDroneDestroyedAfterDelay(3f));
            }
        }

        private IEnumerator NotifyDroneDestroyedAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            ParentCompartment.NotifyDroneIsDestroyed(this);
        }

        private void PlayDrillingEffect()
        {
            drillingVFX.Play();
            drillingSFXAudioSource.PlayOneShot(drillingSFXAudioClips[m_LocalRandom.Next(0, drillingSFXAudioClips.Length)]);
        }

        [JetBrains.Annotations.UsedImplicitly]
        [BehaviorDesigner.Runtime.Tasks.TaskCategory("Traincraft/ScrapperDrone")]
        public class ManipulateDecisionContext : BehaviorDesigner.Runtime.Tasks.Action
        {
            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBehaviour targetScrapperDrone;

            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBool isScrapTargetExists;

            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBool isScrapTargetValid;

            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBool isScrapContainerFull;

            public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
            {
                var scrapperDrone = (ScrapperDrone)targetScrapperDrone.Value;
                Debug.Assert(scrapperDrone != null);
                
                var scrapTargetVein = scrapperDrone.CurrentScrapTargetVein;
                isScrapTargetExists.Value = scrapTargetVein != null;
                isScrapTargetValid.Value = isScrapTargetExists.Value && scrapTargetVein.IsValidToScrap;

                isScrapContainerFull.Value =
                    scrapperDrone.resourceContainerTotalAmount >= scrapperDrone.statComponent.ContainerCapacity;
                
                return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
            }
        }

        [JetBrains.Annotations.UsedImplicitly]
        [BehaviorDesigner.Runtime.Tasks.TaskCategory("Traincraft/ScrapperDrone")]
        public class MoveTowardsTargetAction : BehaviorDesigner.Runtime.Tasks.Action
        {
            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedVector3 position;

            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBehaviour targetScrapperDrone;
            
            [JetBrains.Annotations.UsedImplicitly]
            public bool doesArriveAtDestination;
            
            [JetBrains.Annotations.UsedImplicitly]
            public bool canMoveTowardsTarget;

            private ScrapperDrone m_ScrapperDrone;

            public override void OnStart()
            {
                m_ScrapperDrone = (ScrapperDrone)targetScrapperDrone.Value;
                Debug.Assert(m_ScrapperDrone != null);
            }

            public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
            {
                m_ScrapperDrone.ManipulateMovementProperties(position.Value);

                doesArriveAtDestination = m_ScrapperDrone.m_DoesArriveAtDestination;
                canMoveTowardsTarget = m_ScrapperDrone.m_CanMoveTowardsTarget;
                
                m_ScrapperDrone.RotateTowards();
                m_ScrapperDrone.MoveTowards();
                
                return doesArriveAtDestination
                    ? BehaviorDesigner.Runtime.Tasks.TaskStatus.Success
                    : BehaviorDesigner.Runtime.Tasks.TaskStatus.Running;
            }
        }

        [JetBrains.Annotations.UsedImplicitly]
        [BehaviorDesigner.Runtime.Tasks.TaskCategory("Traincraft/ScrapperDrone")]
        public class DepositScrappedResourceAction : BehaviorDesigner.Runtime.Tasks.Action
        {
            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBehaviour targetScrapperDrone;

            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBool isCurrentScrapTargetValid;
            
            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBool isNewScrapTargetAvailable;

            public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
            {
                var scrapperDrone = (ScrapperDrone)targetScrapperDrone.Value;
                Debug.Assert(scrapperDrone != null);
                
                scrapperDrone.DepositScrappedResource();

                var isCurrentTargetAvailable = ResourceVein.IsVeinValidToScrap(scrapperDrone.CurrentScrapTargetVein);

                if (isCurrentTargetAvailable)
                {
                    isCurrentScrapTargetValid.Value = true;
                    isNewScrapTargetAvailable.Value = false;
                    return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
                }
                
                if (!scrapperDrone.ParentCompartment.TryGetResourceVein(out var newScrapTarget))
                {
                    scrapperDrone.CurrentScrapTargetVein = null;
                    isCurrentScrapTargetValid.Value = false;
                    isNewScrapTargetAvailable.Value = false;
                    
                    return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
                }

                scrapperDrone.CurrentScrapTargetVein = newScrapTarget;
                isCurrentScrapTargetValid.Value = false;
                isNewScrapTargetAvailable.Value = true;
                
                return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
            }
        }

        [JetBrains.Annotations.UsedImplicitly]
        [BehaviorDesigner.Runtime.Tasks.TaskCategory("Traincraft/ScrapperDrone")]
        public class ScrapCurrentTargetAction : BehaviorDesigner.Runtime.Tasks.Action
        {
            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBehaviour targetScrapperDrone;
            
            [JetBrains.Annotations.UsedImplicitly]
            public bool terminatedDueToFullContainer;

            [JetBrains.Annotations.UsedImplicitly]
            public bool terminatedDueToVeinDestroyed;

            private ScrapperDrone m_ScrapperDrone;

            private float m_DelayElapsedTime;

            public override void OnStart()
            {
                m_ScrapperDrone = (ScrapperDrone)targetScrapperDrone.Value;
                Debug.Assert(m_ScrapperDrone != null);

                terminatedDueToFullContainer = false;
                terminatedDueToVeinDestroyed = false;
            }
            
            public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
            {
                if (m_ScrapperDrone.resourceContainerTotalAmount >= m_ScrapperDrone.statComponent.ContainerCapacity)
                {
                    terminatedDueToFullContainer = true;
                    return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
                }

                m_DelayElapsedTime += Time.deltaTime;
                if (m_DelayElapsedTime < m_ScrapperDrone.statComponent.ScrapRate)
                {
                    return BehaviorDesigner.Runtime.Tasks.TaskStatus.Running;
                }
                
                m_DelayElapsedTime = 0f;
                m_ScrapperDrone.PlayDrillingEffect();
                
                return m_ScrapperDrone.TryScrapTargetVein()
                    ? BehaviorDesigner.Runtime.Tasks.TaskStatus.Running
                    : BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
            }
        }

        [JetBrains.Annotations.UsedImplicitly]
        [BehaviorDesigner.Runtime.Tasks.TaskCategory("Traincraft/ScrapperDrone")]
        public class ReturnToParentCompartment : BehaviorDesigner.Runtime.Tasks.Action
        {
            [JetBrains.Annotations.UsedImplicitly]
            public BehaviorDesigner.Runtime.SharedBehaviour targetScrapperDrone;

            public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
            {
                var scrapperDrone = (ScrapperDrone)targetScrapperDrone.Value;
                Debug.Assert(scrapperDrone != null);

                scrapperDrone.DepositScrappedResource();
                scrapperDrone.IsActing = false;
                scrapperDrone.ParentCompartment.StoreDrone(scrapperDrone);
                
                return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
            }
        }
    }
}