using System.Collections.Generic;
using Attributes;
using Data;
using Utility;
using TrainScripts;
using Units.Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameModeManager : SingletonObject<GameModeManager>
    {
        [Header("Circuit Failure Properties")]
        [SerializeField, Disabled] private float circuitFailureTimer = 0;
        [SerializeField, Disabled] private float powerStoneConsumeTimer = 0;
        [SerializeField, Disabled] private float circuitFailureFinalTime;

        [Header("Patrol Squad Spawn-Loop Properties")]
        [SerializeField] private float patrolSquadSpawnCheckInterval = 30f;
        [SerializeField, Disabled] private float patrolSquadSpawnCheckElapsedTime;
        [SerializeField] private MobWavePreset[] patrolSquadPresets;
        [SerializeField] private List<GameObject> patrolSquadMembers = new(16);
        [SerializeField] private float patrolSquadValidationInterval;
        [SerializeField, Disabled] private float patrolSquadValidationElapsedTime;
        [SerializeField, Disabled] private bool isLastSpawnedPatrolSquadAlive;
        [SerializeField] private float patrolSquadSpawnAlertTime = 10f;

        private Train m_Train;

        #region MonoBehaviour Events
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += FindCurrentSceneTrain;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= FindCurrentSceneTrain;
        }

        private void Update()
        {
            var deltaTime = TimeScaleManager.Get.GetDeltaTimeOfTag("Global");
            
            UpdatePatrolSquadSpawnLoop(deltaTime);
            UpdateCircuitFailureEvent(deltaTime);
            UpdatePowerStoneConsumeEvent(deltaTime);
        }
        
        #endregion
        
        #region GameModeManager Implementations

        private void FindCurrentSceneTrain(Scene scene, LoadSceneMode mode)
        {
            m_Train = FindObjectOfType<Train>();
            if (m_Train == null)
            {
                Debug.Log("No Train object exists in this scene");
                return;
            }
            
            SetCircuitFailureTime();
        }
        
        #endregion
        
        #region Patrol Squad Spawn-Loop Implementations

        private void UpdatePatrolSquadSpawnLoop(float deltaTime)
        {
            if (isLastSpawnedPatrolSquadAlive)
            {
                patrolSquadValidationElapsedTime += deltaTime;
                if (patrolSquadValidationElapsedTime >= patrolSquadValidationInterval)
                {
                    patrolSquadValidationElapsedTime = 0f;
                    
                    var isAllPatrolSquadMembersDead = true;
                    for (var i = 0; i < patrolSquadMembers.Count; i++)
                    {
                        if (patrolSquadMembers[i] != null)
                        {
                            isAllPatrolSquadMembersDead = false;
                            break;
                        }
                    }

                    if (isAllPatrolSquadMembersDead)
                    {
                        isLastSpawnedPatrolSquadAlive = false;
                        patrolSquadSpawnCheckElapsedTime = 0f;
                    }
                }
                    
                return;
            }
            
            patrolSquadSpawnCheckElapsedTime += deltaTime;
            if (patrolSquadSpawnCheckElapsedTime < patrolSquadSpawnCheckInterval)
            {
                return;
            }

            patrolSquadSpawnCheckElapsedTime = 0f;
            if (isLastSpawnedPatrolSquadAlive)
            {
                return;
            }
            
            SpawnPatrolSquad();
        }

        private void SpawnPatrolSquad()
        {
            patrolSquadMembers.Clear();
            
            var profile = patrolSquadPresets[UnityEngine.Random.Range(0, patrolSquadPresets.Length)];
            var position = m_Train.transform.position;
            
            var spawnBoundary = Vector3.one * 40f;
            var spawnPoint = Vector3.zero;
            
            foreach (var spawnProfile in profile.spawnProfiles)
            {
                Debug.Assert(spawnProfile.prefab != null, $"Prefab which is referencing from `MobSpawnProfile` is null");
                
                spawnPoint.x = Mathf.Clamp(spawnProfile.localPosition.x, -spawnBoundary.x, spawnBoundary.x);
                spawnPoint.z = Mathf.Clamp(spawnProfile.localPosition.z, -spawnBoundary.z, spawnBoundary.z);
                
                var spawnPosition = position + spawnPoint;
                
                var mobGameObject = Instantiate(spawnProfile.prefab, spawnPosition, spawnProfile.prefab.transform.rotation);
                patrolSquadMembers.Add(mobGameObject.GetComponent<GameObject>());
            }

            isLastSpawnedPatrolSquadAlive = true;
        }
        
        #endregion
        
        #region Circuit Failure Implementations

        private void UpdateCircuitFailureEvent(float deltaTime)
        {
            if (m_Train == null)
            {
                return;
            }

            circuitFailureTimer += deltaTime;

            if (circuitFailureTimer >= circuitFailureFinalTime)
            {
                circuitFailureTimer = 0;
                m_Train.RandomCarCircuitFailure();
            }
        }

        private void UpdatePowerStoneConsumeEvent(float deltaTime)
        {
            if (m_Train == null || !m_Train.CanMove)
            {
                return;
            }

            powerStoneConsumeTimer += deltaTime;

            if (powerStoneConsumeTimer >= Database.GlobalBalanceSetting?.powerStoneConsumeInterval)
            {
                powerStoneConsumeTimer = 0;
                m_Train.PowerStoneConsume();
            }
        }

        public void SetCircuitFailureTime()
        {
            circuitFailureFinalTime = Database.GlobalBalanceSetting.circuitFailureInterval *
                                      (m_Train.StatComponent.CarSafety == 0 ? 1 : m_Train.StatComponent.CarSafety);
        }
        
        #endregion
    }
}