using System;
using UnityEngine;

using Attributes;
using Data;

namespace TrainScripts
{
    public partial class Car
    {
        #region Circuit Failure & Repair References

        [Space, Header("Circuit References")]
        [SerializeField, Disabled] private bool m_CircuitFailure;
        [SerializeField, Disabled] private float m_CircuitFailureTime;
        [SerializeField, Disabled] private float m_CircuitFailureElapsedTime;

        private VFX m_CircuitFailureVFX;
        private bool m_CircuitFailureStart;
        private int m_CircuitRepairAmount;
        private float m_CircuitRepairTime;
       
        public bool CircuitFailure
        {
            get => m_CircuitFailure;
            set
            {
                m_CircuitFailure = value;
                if (m_CircuitFailure)
                {
                    OnCircuitFailure?.Invoke();
                }
            }
        }
        
        private Action OnCircuitFailure;

        #endregion

        #region Circuit Failure & Repair Implementations

        public virtual void CheckCircuitFailure()
        {
            if (isCarDestroyed)
            {
                return;
            }
            
            var currentHealthPercentage = statComponent.CurrentHealth / statComponent.MaxHealth;
            currentHealthPercentage = Mathf.Round(currentHealthPercentage * 100f) / 100f;
            if (currentHealthPercentage > Database.GlobalBalanceSetting.circuitFailureHealthPercentage || m_CircuitFailure)
            {
                m_CircuitFailureTime = 0f;
                m_CircuitFailureElapsedTime = 0f;
                return;
            }
            m_CircuitFailureTime = ((Database.GlobalBalanceSetting.circuitFailureMultiplier * currentHealthPercentage * 100f) + Database.GlobalBalanceSetting.circuitFailureAddition) 
                * (parentTrain.StatComponent.CarSafety == 0 ? 1 : parentTrain.StatComponent.CarSafety);
            m_CircuitFailureStart = true;
        }

        // Overload method for `CheckCircuitFailure()` to bind with `UnitStatComponent.OnHealthPointChanged` event.
        private void CheckCircuitFailure(float value)
        {
            CheckCircuitFailure();
        }

        protected virtual void CarCircuitFailure()
        {
            m_CircuitFailureStart = false;
            m_CircuitFailureTime = 0f;
            m_CircuitFailureElapsedTime = 0f;
            switch (statComponent.UpgradeLevel)
            {
                case 2:
                    m_CircuitRepairAmount = 2;
                    break;
                case 3:
                    m_CircuitRepairAmount = 2;
                    break;
                case 4:
                    m_CircuitRepairAmount = 3;
                    break;
                default:
                    m_CircuitRepairAmount = 1;
                    break;
            }
            m_CircuitFailureVFX?.PlayVFX();
            carCircuitRepairItem.onClick.RemoveAllListeners();
            carCircuitRepairItem.onClick.AddListener(() => RepairCircuit());
            carCircuitRepairItem.gameObject.SetActive(true);
            OnChangeCircuitRepairItem(parentTrain.Inventory);
            if (this is not EngineCompartment)
            {
                CarStopWorking();
            }
            Debug.Log($"This car({name}) circuit is broken! The car number is {parentTrain.cars.IndexOf(this)}");
        }

        protected virtual void CarCircuitRepair()
        {
            m_CircuitFailure = false;
            CheckCircuitFailure();
            if (this is not EngineCompartment)
            {
                CarStartWorking();
            }
            m_CircuitFailureVFX?.StopVFX();
            Debug.Log($"The circuit of this car({name}) was repaired. The car number is {parentTrain.cars.IndexOf(this)}");
        }

        [ContextMenu("Repair Circuit")]
        public void RepairCircuit()
        {
            if (!m_CircuitFailure || m_DelayedOperation != null)
            {
                return;
            }

            parentTrain.ConsumeResourceToInventory(ResourceType.Circuit, m_CircuitRepairAmount);
            m_CircuitRepairTime = Database.GlobalBalanceSetting.circuitDefaultRepairTime + statComponent.UpgradeLevel;
            carCircuitRepairItem.gameObject.SetActive(false);

            StartOperationAfterDelay(m_CircuitRepairTime, OperationType.RepairCircuit, CarCircuitRepair);
        }

        private void UpdateCircuitFailureTime(float deltaTime)
        {
            if (!m_CircuitFailureStart)
            {
                return;
            }

            m_CircuitFailureElapsedTime += deltaTime;

            if (m_CircuitFailureElapsedTime >= m_CircuitFailureTime)
            {
                CircuitFailure = true;
            }
        }

        public void OnChangeCircuitRepairItem(Inventory inventory)
        {
            bool isAvailable = parentTrain.CheckAvailabilityResource(ResourceType.Circuit, m_CircuitRepairAmount);
            carCircuitRepairItem.SetRepairAmountColor(isAvailable);
            carCircuitRepairItem.RepairAmount = m_CircuitRepairAmount.ToString();
        }

        protected virtual void StopCircuitFailure(bool resetAllState)
        {
            CircuitFailure = false;
            m_CircuitFailureStart = false;

            if (resetAllState)
            {
                m_CircuitFailureTime = 0f;
                m_CircuitFailureElapsedTime = 0f;
                carCircuitRepairItem.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}