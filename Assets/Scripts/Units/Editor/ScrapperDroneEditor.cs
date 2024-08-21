using Units.Stats;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Units.Drones
{
    [CustomEditor(typeof(ScrapperDrone))]
    public class ScrapperDroneEditor : Editor
    {
        private ScrapperDrone Target { get; set; }
        
        private Transform Transform { get; set; }
        
        private ScrapperDroneStatComponent StatComponent { get; set; }
        
        private void OnEnable()
        {
            Target = (ScrapperDrone)target;
            Transform = Target.GetComponent<Transform>();
            StatComponent = Target.GetComponent<ScrapperDroneStatComponent>();
        }

        private void OnSceneGUI()
        {
            var transform = Transform;
            var position = transform.position;
            var sightRight = Quaternion.AngleAxis(StatComponent.TurnThresholdAngle * -0.5f, Vector3.up) * transform.forward;
            var sightLeft = Quaternion.AngleAxis(StatComponent.TurnThresholdAngle * 0.5f, Vector3.up) * transform.forward;
            
            Handles.color = Color.green;
            Handles.DrawWireArc(position, Vector3.up, Vector3.right, 360, StatComponent.AttackRange, 3f);
            
            Handles.color = Color.blue;
            Handles.DrawWireArc(position, Vector3.up, sightRight, StatComponent.TurnThresholdAngle, StatComponent.AttackRange, 3f);
            Handles.DrawLine(position, position + sightRight, 3f);
            Handles.DrawLine(position, position + sightLeft, 3f);
        }
    }
}