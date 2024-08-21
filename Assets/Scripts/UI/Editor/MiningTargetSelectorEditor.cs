using UnityEditor;

namespace UI
{
    // [CustomEditor(typeof(MiningTargetSelector))]
    // public class MiningTargetSelectorEditor : Editor
    // {
    //     private MiningTargetSelector Target { get; set; }
    //
    //     private void OnEnable()
    //     {
    //         Target = (MiningTargetSelector)target;
    //     }
    //
    //     private void OnSceneGUI()
    //     {
    //         if (Target == null || !Target.enabled || !Target.IsSelectingAvailable)
    //         {
    //             return;
    //         }
    //         
    //         var position = Target.SelectRaycastPoint;
    //         var radius = Target.SelectRadius;
    //
    //         Handles.color = UnityEngine.Color.red;
    //         Handles.DrawWireDisc(position, UnityEngine.Vector3.up, radius, 3f);
    //     }
    // }
}