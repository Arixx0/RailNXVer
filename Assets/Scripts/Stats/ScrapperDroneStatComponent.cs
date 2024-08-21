using UnityEngine;

namespace Units.Stats
{
    public class ScrapperDroneStatComponent : UnitStatComponent
    {
        [Header("Scrapper Drone Stats"), SerializeField]
        protected StatVariable scrapRate;

        [SerializeField]
        protected StatVariable scrapAmountPerAction;

        [SerializeField]
        protected StatVariable containerCapacity;

        public float ScrapRate => scrapRate.Value;

        public float ScrapAmountPerAction => scrapAmountPerAction.Value;

        public float ContainerCapacity => containerCapacity.Value;

        public override void Setup(bool reset = false)
        {
            base.Setup(reset);
            
            scrapRate.Setup();
            scrapAmountPerAction.Setup();
            containerCapacity.Setup();
        }
    }
}