using Data;
using Environments;
using TrainScripts;
using UI;
using Utility;

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Manager
{
    public class MockupGameModeManager : SingletonObject<MockupGameModeManager>
    {
        public Transform root;
        public NavMeshSurface navMesh;
        public MapSector mapSector;
        public RailPath railPath;
        public Train train;

        [Space]
        public StageGenerationSettings stageGenerationSettings;
        public MobSpawner mobSpawner;
        public ResourceVeinSpawner resourceVeinSpawner;

        [Space]
        public ShopUI shopUI;
            
        private bool m_EnableGUIMenu = false;
        private bool m_EnableTrainManagementMenu = false;
        private bool m_EnableEventSpawnMenu = false;
        private RoomEventType m_SelectedRoomEventType = RoomEventType.None;
        private int m_LastSpawnMobPresetIndex = 0;

        private readonly System.Random m_Random = new();
        
        protected override void Awake()
        {
            base.Awake();
            
            Setup();
            
            train.Setup();
            train.AlignToCenter();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Enable GUI Menu"))
            {
                m_EnableGUIMenu = !m_EnableGUIMenu;
            }

            if (m_EnableGUIMenu)
            {
                return;
            }
            
            DrawTrainManagementMenu();
            DrawEventSpawnMenu();
        }

        public void Setup()
        {
            mapSector.CreateSectorPaths();
            navMesh.UpdateNavMesh(navMesh.navMeshData);
            
            railPath.ClearPaths();
            railPath.AddPath(mapSector.Path.MainPath);

            train.AlignToCenter();
        }

        private void DrawTrainManagementMenu()
        {
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Draw Train Management Menu"))
            {
                m_EnableTrainManagementMenu = !m_EnableTrainManagementMenu;
            }
            
            if (!m_EnableTrainManagementMenu)
            {
                GUILayout.EndHorizontal();
                return;
            }
            
            if (GUILayout.Button("Align Train at Center"))
            {
                train.AlignToCenter();
            }
            
            GUILayout.EndHorizontal();
        }

        private void DrawEventSpawnMenu()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Event Spawn Menu"))
            {
                m_EnableEventSpawnMenu = !m_EnableEventSpawnMenu;
            }
            
            if (!m_EnableEventSpawnMenu)
            {
                GUILayout.EndHorizontal();
                return;
            }

            if (GUILayout.Button("Reset All Event Context"))
            {
                ResetAllStageEventContexts();
            }

            foreach (var eventType in Enum.GetValues(typeof(RoomEventType)))
            {
                if (GUILayout.Button(eventType.ToString()))
                {
                    m_SelectedRoomEventType = (RoomEventType) eventType;
                }
            }
            
            GUILayout.EndHorizontal();

            switch (m_SelectedRoomEventType)
            {
                case RoomEventType.ChopShop:
                    break;
                case RoomEventType.ResourcePiles:
                    if (GUILayout.Button("Spawn Resource Veins"))
                    {
                        var resourceAmount = new Dictionary<ResourceType, ResourceVeinData>(8)
                        {
                            // the power stone should be spawned always
                            {
                                ResourceType.PowerStone,
                                new ResourceVeinData
                                {
                                    resourceType = ResourceType.PowerStone,
                                    amount = (int)stageGenerationSettings.inGameResourcesSpawnArgs
                                        .First(e => e.resourceType == ResourceType.PowerStone)
                                        .spawnAmountRange.FromRandom(m_Random)
                                }
                            }
                        };

                        // generate resource spawn context for the room
                        var resourceSpawnCount = m_Random.Next(1, stageGenerationSettings.spawnableResourceOfTypeCount);
                        foreach (var arg in stageGenerationSettings.inGameResourcesSpawnArgs)
                        {
                            switch (arg.resourceType)
                            {
                                case ResourceType.Iron:
                                case ResourceType.Titanium:
                                case ResourceType.Mythrill:
                                    resourceAmount.Add(arg.resourceType,
                                        new ResourceVeinData()
                                        {
                                            resourceType = arg.resourceType,
                                            amount = (int)arg.spawnAmountRange.FromRandom(m_Random)
                                        });
                                    break;
                                default: continue;
                            }

                            resourceSpawnCount -= 1;

                            if (resourceSpawnCount <= 0)
                            {
                                break;
                            }
                        }

                        ResetAllStageEventContexts();
                        resourceVeinSpawner.Spawn(mapSector, resourceAmount.Values.ToList());
                    }
                    break;
                case RoomEventType.NormalEnemyWave:
                case RoomEventType.EliteEnemyWave:
                case RoomEventType.EnemyOutpost:
                    if (GUILayout.Button("Spawn Random Mob Preset"))
                    {
                        m_LastSpawnMobPresetIndex =
                            UnityEngine.Random.Range(0, stageGenerationSettings.mobSpawnProfiles.Length);
                        
                        ResetAllStageEventContexts();
                        mobSpawner.Spawn(stageGenerationSettings.mobSpawnProfiles[m_LastSpawnMobPresetIndex]);
                        mobSpawner.ActivateMobWave();
                    }
                    break;
                case RoomEventType.ScrapPiles:
                    break;
                case RoomEventType.SpaceCenter:
                    break;
                case RoomEventType.ShopKeeper:
                    if (GUILayout.Button("Execute Shop Keeper Event"))
                    {
                        ResetAllStageEventContexts();
                        train.SetMovementState(false);
                        shopUI.AssignProperties(stageGenerationSettings.shopKeeperItemsDefinition, () => train.SetMovementState(true));
                        shopUI.Show(stageGenerationSettings.shopKeeperItemsDefinition, () => train.SetMovementState(true));
                    }
                    break;
            }
        }

        private void ResetAllStageEventContexts()
        {
            resourceVeinSpawner.DestroySpawnedVeins();
            mobSpawner.DestroySpawnedMobs();
        }
    }
}