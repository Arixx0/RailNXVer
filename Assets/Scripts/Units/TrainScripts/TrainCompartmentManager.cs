// ReSharper disable CheckNamespace

using Attributes;
using Utility;
using Data;

using System.Collections.Generic;
using UI;
using UnityEngine;

namespace TrainScripts
{
    public partial class Train
    {
        [Header("Train Compartment Managing Options")]
        public List<Car> cars = new(16);
        public int maxSpawnableCars = 16;
        public float gapBetweenCars = 0.1f;
        public List<Car> carPrefabsForInitialSpawn = new(16);
        public JunkStateCompartment junkStateCompartmentPrefab;

        [Space]
        [SerializeField, Disabled] private bool m_ElectricPowerOverload;
        [SerializeField, Disabled] private float m_TrainCurrentElectricPowerGeneration;
        [SerializeField, Disabled] private float m_TrainCurrentElectricPowerUsage;

        private AudioClip m_OverLoadSFX;
        private AudioClip m_RestoreSFX;

        public bool ElectricPowerOverload => m_ElectricPowerOverload;

        public float TrainCurrentElectricPowerGeneration => m_TrainCurrentElectricPowerGeneration;

        public float TrainCurrentElectricPowerUsage => m_TrainCurrentElectricPowerUsage;
        
        public int LeftOverCarSlots => maxSpawnableCars - cars.Count;

        public Vector3 TailSpawnPosition
        {
            get
            {
                var distanceDelta = m_TravelDistanceOnPath - totalTrainLength - cars[^1].UnitLengthHalf - gapBetweenCars;
                railPath.GetTransformAtDistance(distanceDelta, out var pos, out var rot);

                return pos;
            }
        }
        
#if UNITY_EDITOR
        [ContextMenu("Force Align")]
        public void ForceAlign()
        {
            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);
        }
#endif
        
#if UNITY_EDITOR
        public void AlignToCenter()
        {
            var pathDistance = railPath.TotalLength * 0.5f + validTrainLength * 0.5f;
            m_TotalTravelDistance
                = m_TravelDistanceOnPath
                = AlignTrainFromDistanceOnPath(pathDistance);
        }
#endif

#if UNITY_EDITOR
        [ContextMenu("Destroy All Spawned")]
        public void DestroyAllSpawned()
        {
            for (var i = cars.Count - 1; i >= 0; i -= 1)
            {
                if (cars[i] == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(cars[i].gameObject);
                }
                else
                {
                    DestroyImmediate(cars[i].gameObject);
                }
            }
            
            cars.Clear();
        }
#endif
        
        public void AdjustMaxSpawnableCars(int amount)
        {
            maxSpawnableCars = Mathf.Clamp(maxSpawnableCars + amount, 1, 32);
        }

        private float AlignTrainFromDistanceOnPath(float distanceOnPath, bool alignFromBack = false)
        {
#if UNITY_EDITOR
            // if no path is assigned, align from transform point.
            if (railPath == null)
            {
                OnValidate();
                
                var position = CachedTransform.position;
                var backward = CachedTransform.forward * -1f;
                var forwardRotation = Quaternion.LookRotation(CachedTransform.forward, Vector3.up);
                
                for (var i = 0; i < cars.Count; i += 1)
                {
                    if (i > 0)
                    {
                        position += backward * (cars[i].UnitLengthHalf + gapBetweenCars);
                    }
                    
                    cars[i].CachedTransform.SetPositionAndRotation(position, forwardRotation);
                    
                    position += backward * cars[i].UnitLengthHalf;
                }
                
                return 0f;
            }
#endif

            var trainPivotDistance = distanceOnPath + (alignFromBack ? validTrainLength : 0f);
            var distanceDelta = trainPivotDistance;

            if (syncTrainTransformToPath)
            {
                railPath.GetTransformAtDistance(distanceDelta, out var position, out var rotation);
                rotation.ResetZAxis();
                
                CachedTransform.SetPositionAndRotation(position, rotation);
            }
            
            for (var i = 0; i < cars.Count; i += 1)
            {
                if (i > 0 || regardTrainAsAlignTarget)
                {
                    distanceDelta -= cars[i].UnitLengthHalf + gapBetweenCars;
                }

                railPath.GetTransformAtDistance(distanceDelta, out var position, out var rotation);
                rotation.ResetZAxis();
                
                cars[i].CachedTransform.SetPositionAndRotation(position, rotation);

                distanceDelta -= cars[i].UnitLengthHalf;
            }

            return trainPivotDistance;
        }

        public void RemoveCarFromManagedList(Car car)
        {
            if (cars.Remove(car))
            {
                AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);
            }
            
            statComponent.RemoveChainedStatComponent(car.StatComponent);

            if (cameraController.FollowTarget == car.CachedTransform)
            {
                var newSelectedIndex = Mathf.Clamp(m_SelectedCarIndex - 1, 0, cars.Count - 1);
                SetSelectedCar(newSelectedIndex);
            }
        }
        
        public void ReplaceCarFromManagedList(Car oldCar, Car newCar)
        {
            var index = cars.IndexOf(oldCar);
            if (index < 0)
            {
                return;
            }
            
            cars[index] = newCar;
            newCar.CachedTransform.SetParent(CachedTransform);
            
            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);

            if (cameraController.FollowTarget == oldCar.CachedTransform)
            {
                cameraController.FollowTarget = newCar.CachedTransform;
            }
            
            Destroy(oldCar.gameObject);
        }

        public void SpawnCar(Car prefab)
        {
            if (cars.Count >= maxSpawnableCars)
            {
                return;
            }
            
            var instance = Instantiate(prefab, CachedTransform);
            instance.Setup(this);
            
            cars.Add(instance);
            statComponent.AddChainedStatComponent(instance.StatComponent);
            
            UpdateTrainStats();
            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);
        }

        public void SpawnCar(Car prefab, float constructionDelay)
        {
            if (cars.Count >= maxSpawnableCars)
            {
                return;
            }
            
            var instance = Instantiate(prefab, CachedTransform);
            instance.Setup(this);
            instance.StartOperationAfterDelay(constructionDelay, OperationType.Build);
            cars.Add(instance);
            
            statComponent.AddChainedStatComponent(instance.StatComponent);
            
            UpdateTrainStats();
            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);
        }

        public void SpawnCars(List<Car> prefabs)
        {
#if UNITY_EDITOR
            m_CachedTransform = m_CachedTransform == null ? transform : m_CachedTransform;
#endif
            
            foreach (var prefab in prefabs)
            {
                var instance = Instantiate(prefab, CachedTransform);
                instance.Setup(this);
                
                cars.Add(instance);
                statComponent.AddChainedStatComponent(instance.StatComponent);
            }
            
            UpdateTrainStats();
            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);
        }

        public void CheckTrainElectricPower()
        {
            m_TrainCurrentElectricPowerGeneration = 0;
            m_TrainCurrentElectricPowerUsage = 0;
            foreach (var car in cars)
            {
                m_TrainCurrentElectricPowerGeneration += car.CurrentElectricPowerGeneration;
                m_TrainCurrentElectricPowerUsage += car.CurrentElectricPowerUsage;
            }

            var currentOverload = m_ElectricPowerOverload;
            m_ElectricPowerOverload = m_TrainCurrentElectricPowerUsage > m_TrainCurrentElectricPowerGeneration ? true : false;

            foreach (var car in cars)
            {
                var target = car is EngineCompartment ? car.ParentTrain.StatComponent.StatusEffectManager : car.StatComponent.StatusEffectManager;
                if (m_ElectricPowerOverload)
                {
                    car.ElectricPowerBuffRemove(target);
                    car.CarStopWorking();
                }
                else
                {
                    car.ElectricPowerBuffAdd(target);
                    car.CarStartWorking();
                }
                if (!currentOverload && m_ElectricPowerOverload) // release overload -> overload
                {
                    car.OverLoadVFX.PlayVFX();
                    trainAudioSource.clip = m_OverLoadSFX;
                    trainAudioSource.Play();
                }
                else if (currentOverload && !m_ElectricPowerOverload) // overload -> release overload
                {
                    car.OverLoadVFX.StopVFX();
                    trainAudioSource.clip = m_RestoreSFX;
                    trainAudioSource.Play();
                }
                car.UpdateCarDetailValue();
            }
            hud.UpdateHUDItemValue();
            if (!currentOverload && m_ElectricPowerOverload)
            {
                noticeCompositor.InvokeNoticeEvent(NoticeType.Notice, "Electric_Overload", "Venus");
            }
        }

        public void BeginReorderCar(Car car)
        {
            var carIndex = cars.IndexOf(car);

            if (carIndex == -1)
            {
                return;
            }

            m_SelectedCarIndex = carIndex;
            m_ReorderInputHandler.Activate(this);
        }
        
        private void ReorderCar(int originalIndex, int targetIndex)
        {
            if (originalIndex == targetIndex ||
                originalIndex == 0 ||
                targetIndex == 0)
            {
                return;
            }

            var originalCar = cars[originalIndex];
            cars.RemoveAt(originalIndex);
            cars.Insert(targetIndex, originalCar);
            hud.CreateCarControlPanel();
            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);
        }

        public void ReplaceCarInstance(Car original, Car replacement)
        {
            var index = cars.IndexOf(original);
            if (index == -1)
            {
                return;
            }

            cars[index] = replacement;
            replacement.CachedTransform.SetParent(CachedTransform);
            
            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);

            if (index == m_SelectedCarIndex)
            {
                cameraController.FollowTarget = replacement.CachedTransform;
                replacement.SetSelectedStatus(true);
            }
            else
            {
                replacement.SetSelectedStatus(false);
            }
        }

        public void SwitchToJunkState(Car car)
        {
            var index = cars.IndexOf(car);
            if (index == -1)
            {
                return;
            }
            
            var junkInstance = Instantiate(junkStateCompartmentPrefab, CachedTransform);
            junkInstance.ActualInstance = car;
            junkInstance.Setup(this);
            
            cars[index] = junkInstance;

            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);

            if (index == m_SelectedCarIndex)
            {
                cameraController.FollowTarget = junkInstance.CachedTransform;
                junkInstance.SetSelectedStatus(true);
            }
            else
            {
                junkInstance.SetSelectedStatus(false);
            }
        }
    }
}