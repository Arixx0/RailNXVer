using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Environments
{
    [CustomEditor(typeof(StageGenerator))]
    public class StageGeneratorEditor : Editor
    {
        private static readonly Dictionary<RoomEventType, string> RoomEventTypeIcons = new()
        {
            { RoomEventType.ChopShop, "Icons/icon_map_chopshop.png" },
            { RoomEventType.ResourcePiles, "Icons/icon_map_resourceveins.png" },
            { RoomEventType.NormalEnemyWave, "Icons/icon_map_enemywave.png" },
            { RoomEventType.EliteEnemyWave, "Icons/icon_map_elitewave.png" },
            { RoomEventType.EnemyOutpost, "Icons/icon_map_outpost.png" },
            { RoomEventType.ScrapPiles, "Icons/icon_map_scrappile.png" },
            { RoomEventType.SpaceCenter, "Icons/icon_map_satelite.png" },
            { RoomEventType.ShopKeeper, "Icons/icon_map_shopkeeper.png" },
            { RoomEventType.BossWave, "Icons/icon_map_boss.png" },
            { RoomEventType.InitialRoom, "Icons/icon_map_startpoint.png" },
        };

        private const string NormalRoadRoomIcon = "Icons/icon_map_normalroad.png";

        private const string CrossRoadRoomIcon = "Icons/icon_map_crossroad.png";

        private static Gradient GridGizmosColorGradient;

        private static Gradient PathingGizmosColorGradient;
        
        private Editor m_StageGenerationSettingsEditor;

        private bool m_FoldoutStageGenerationSettings;
        
        private StageGenerator Target { get; set; }
        
        private void OnEnable()
        {
            Target = (StageGenerator)target;

            GridGizmosColorGradient = new Gradient();
            GridGizmosColorGradient.SetKeys(
                new GradientColorKey[]
                {
                    new (new Color(0.145f, 0.262f, 0.212f), 0.0f),
                    new (new Color(0.855f, 0.827f, 0.745f), 1.0f)
                },
                new GradientAlphaKey[]
                {
                    new (1.0f, 0.0f),
                    new (1.0f, 1.0f)
                });

            PathingGizmosColorGradient = new Gradient();
            PathingGizmosColorGradient.SetKeys(
                new GradientColorKey[]
                {
                    new (new Color(0.027f, 0.059f, 0.169f), 0.0f),
                    new (new Color(0.573f, 0.565f, 0.765f), 1.0f)
                },
                new GradientAlphaKey[]
                {
                    new (1.0f, 0.0f),
                    new (1.0f, 1.0f)
                });
        }

        private void OnValidate()
        {
            if (Target.Settings != null)
            {
                CreateCachedEditor(Target.Settings, null, ref m_StageGenerationSettingsEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Stage Data"))
            {
                Target.GeneratePaths();
            }

            if (GUILayout.Button("Preview Stage") &&
                Target.Rooms.Count != 0)
            {
                GeneratedStagePreviewer.ShowPreview(Target.Rooms, Target.Settings);
            }

            if (m_StageGenerationSettingsEditor != null)
            {
                m_FoldoutStageGenerationSettings = EditorGUILayout.InspectorTitlebar(
                    m_FoldoutStageGenerationSettings, m_StageGenerationSettingsEditor);
                if (m_FoldoutStageGenerationSettings)
                {
                    m_StageGenerationSettingsEditor.OnInspectorGUI();
                }
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        private static void DrawStageSectorIcon(StageGenerator src, GizmoType gizmoType)
        {
            foreach (var sector in src.MapSectors)
            {
                var iconName = sector.SectorType switch
                {
                    MapSectorType.NormalRoad => NormalRoadRoomIcon,
                    MapSectorType.CrossRoad => CrossRoadRoomIcon,
                    _ => RoomEventTypeIcons[src.Rooms[sector.RoomIndex].roomEventType]
                };

                Gizmos.DrawIcon(sector.transform.position + Vector3.up * 1f, iconName, false);
            }
        }
    }
}