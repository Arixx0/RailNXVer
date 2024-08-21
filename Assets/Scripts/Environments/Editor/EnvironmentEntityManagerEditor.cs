using UnityEditor;
using UnityEngine;

namespace Environments
{
    [CustomEditor(typeof(EnvironmentEntityManager))]
    public class EnvironmentEntityManagerEditor : Editor
    {
        public override bool HasPreviewGUI()
        {
            var entityManager = (EnvironmentEntityManager)target;
            return entityManager.NoiseMapTex2D != null;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            
            var entityManager = (EnvironmentEntityManager)target;
            if (entityManager.NoiseMapTex2D != null)
            {
                var rect = r;
                
                rect.width = Mathf.Min(r.width, r.height);
                rect.height = rect.width;
                
                rect.x = r.x + (r.width * 0.5f - rect.width * 0.5f);
                rect.y = r.y + (r.height * 0.5f - rect.height * 0.5f);
                
                GUI.DrawTexture(rect, entityManager.NoiseMapTex2D);
            }
        }
    }
}