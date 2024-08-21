using System.Collections;
using UnityEngine;

namespace TrainScripts
{
    public class CarDelayedOperation : IEnumerator
    {
        protected Car operatingAgent;
        protected float delay;
        protected float elapsedTime;
        protected OperationType operationType;

        public OperationType OperationType => operationType;

        public float RemainingTime => delay - elapsedTime;

        public event OnTaskUpdateDelegate OnTaskUpdate;
        
        public event OnTaskCompleteDelegate OnTaskComplete;
        
        public CarDelayedOperation(Car operatingAgent, float delay, OperationType operationType = OperationType.None)
        {
            this.operatingAgent = operatingAgent;
            this.delay = delay;
            this.operationType = operationType;
            elapsedTime = 0;
        }

        public virtual bool MoveNext()
        {
            var deltaTime = TimeScaleManager.Get.GetDeltaTimeOfTag(operatingAgent.tag);
            elapsedTime += deltaTime;
            
            OnTaskUpdate?.Invoke(elapsedTime / delay);

            if (elapsedTime >= delay)
            {
                OnTaskComplete?.Invoke();
            }
            
            return elapsedTime <= delay;
        }

        public void Reset()
        {
            operatingAgent = null;
            delay = 0;
            elapsedTime = 0;
            operationType = OperationType.None;
        }

        public void Reset(Car operatingAgent, float delay, OperationType operationType)
        {
            this.operatingAgent = operatingAgent;
            this.delay = delay;
            elapsedTime = 0;
            this.operationType = operationType;
        }

        public object Current => null;

        public delegate void OnTaskCompleteDelegate();

        public delegate void OnTaskUpdateDelegate(float progress);
    }

    public enum OperationType
    {
        None = -1,
        Build,
        Demolish,
        RepairDurability,
        RepairCircuit,
        ElectricPowerChange,
        Upgrade,
        Restore
    }
}