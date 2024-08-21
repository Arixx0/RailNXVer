using UnityEditor;
using UnityEngine;

namespace UI
{
    [CustomPropertyDrawer(typeof(DetailLayer))]
    public class HUDDetailLayerDrawer : PropertyDrawer
    {
        private readonly string[] detailLayerNames = { "Default Layer", "Power Stone Layer" , "Electric Power Layer", "Power Stone Detail Layer" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty detailItemsProperty = property.FindPropertyRelative("detailItems");
            if (detailItemsProperty == null) return;

            int index = GetIndexFromPropertyPath(property.propertyPath);
            string layerName = detailLayerNames.Length > index ? detailLayerNames[index] : $"Layer {index + 1}";

            EditorGUI.PropertyField(position, detailItemsProperty, new GUIContent(layerName), true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty detailItemsProperty = property.FindPropertyRelative("detailItems");
            return EditorGUI.GetPropertyHeight(detailItemsProperty, label, true);
        }

        private int GetIndexFromPropertyPath(string propertyPath)
        {
            string[] pathParts = propertyPath.Split('[', ']');
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (int.TryParse(pathParts[i], out int index))
                {
                    return index;
                }
            }
            return 0;
        }
    }
}