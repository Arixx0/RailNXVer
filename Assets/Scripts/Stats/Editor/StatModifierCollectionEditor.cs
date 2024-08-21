using System.Linq;
using Units.Stats;
using UnityEditor;

[CustomEditor(typeof(StatModifierCollection), true)]
public class StatModifierCollectionEditor : Editor
{
    private static readonly string[] StatModifierCollectionProperties = new string[]
    {
        "m_Script",
        "displayName",
        "description",
        "guid",
        "applicableUnitTag",
        "refreshOnApply",
        "enableHealthPointModifier",
        "healthPointModifier",
        "enableArmorPointModifier",
        "armorPointModifier",
        "enableMoveSpeedModifier",
        "moveSpeedModifier",
        "enableRotateSpeedModifier",
        "rotateSpeedModifier",
        "enableAttackDamageModifier",
        "attackDamageModifier",
        "enableArmorPierceModifier",
        "armorPierceModifier",
        "enableAttackSpeedModifier",
        "attackSpeedModifier",
        "enableAttackRangeModifier",
        "attackRangeModifier",
        "enableFuelEfficiencyModifier",
        "fuelEfficiencyModifier",
        "enableCarSafetyModifier",
        "carSafetyModifier"
    };
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("guid"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("applicableUnitTag"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("refreshOnApply"));

        EditorGUILayout.Space();

        var enableHealthPointModifier = serializedObject.FindProperty("enableHealthPointModifier");
        EditorGUILayout.PropertyField(enableHealthPointModifier);
        if (enableHealthPointModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("healthPointModifier"));
        }

        var enableArmorPointModifier = serializedObject.FindProperty("enableArmorPointModifier");
        EditorGUILayout.PropertyField(enableArmorPointModifier);
        if (enableArmorPointModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("armorPointModifier"));
        }

        var enableMoveSpeedModifier = serializedObject.FindProperty("enableMoveSpeedModifier");
        EditorGUILayout.PropertyField(enableMoveSpeedModifier);
        if (enableMoveSpeedModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveSpeedModifier"));
        }

        var enableRotateSpeedModifier = serializedObject.FindProperty("enableRotateSpeedModifier");
        EditorGUILayout.PropertyField(enableRotateSpeedModifier);
        if (enableRotateSpeedModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateSpeedModifier"));
        }

        var enableAttackDamageModifier = serializedObject.FindProperty("enableAttackDamageModifier");
        EditorGUILayout.PropertyField(enableAttackDamageModifier);
        if (enableAttackDamageModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackDamageModifier"));
        }

        var enableArmorPierceModifier = serializedObject.FindProperty("enableArmorPierceModifier");
        EditorGUILayout.PropertyField(enableArmorPierceModifier);
        if (enableArmorPierceModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("armorPierceModifier"));
        }

        var enableAttackSpeedModifier = serializedObject.FindProperty("enableAttackSpeedModifier");
        EditorGUILayout.PropertyField(enableAttackSpeedModifier);
        if (enableAttackSpeedModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackSpeedModifier"));
        }

        var enableAttackRangeModifier = serializedObject.FindProperty("enableAttackRangeModifier");
        EditorGUILayout.PropertyField(enableAttackRangeModifier);
        if (enableAttackRangeModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRangeModifier"));
        }

        var enableFuelEfficiencyModifier = serializedObject.FindProperty("enableFuelEfficiencyModifier");
        EditorGUILayout.PropertyField(enableFuelEfficiencyModifier);
        if (enableFuelEfficiencyModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fuelEfficiencyModifier"));
        }

        var enableCarSafetyModifier = serializedObject.FindProperty("enableCarSafetyModifier");
        EditorGUILayout.PropertyField(enableCarSafetyModifier);
        if (enableCarSafetyModifier.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("carSafetyModifier"));
        }

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true);

        while (property.NextVisible(false))
        {
            if (serializedObject.targetObject is StatusEffects.Environments.EnvironmentStatusEffect && property.name == "duration")
            {
                continue;
            }
            
            if (!StatModifierCollectionProperties.Contains(property.name))
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}