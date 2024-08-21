using System;
using UnityEditor;

namespace UI
{
    [CustomEditor(typeof(ContextMenuUI))]
    public class ContextMenuUIEditor : Editor
    {
        private ContextMenuUI Target { get; set; }
        
        private void OnEnable()
        {
            Target = target as ContextMenuUI;
        }

        private void OnSceneGUI()
        {
            if (!Target.IsEnabled)
            {
                return;
            }
            
            Handles.Label(Target.transform.position, Target.InputAngularDirection.ToString());

            foreach (var item in Target.ActiveContextMenuItems)
            {
                Handles.Label(item.transform.position, item.AngularPosition.ToString());
            }
        }
    }
}
