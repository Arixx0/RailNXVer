using CameraUtility;
using PathCreation;
using UI;
using UnityEngine;

namespace Environments
{
    public class PassageSelectEventExecutor : MonoBehaviour
    {
        [SerializeField]
        private StageSectorEventTrigger trigger;

        private IPassageSelectEventDataProvider m_DataProvider;

        private float m_GlobalTimeScaleCache;

        private void OnTriggerPerformed()
        {
            m_GlobalTimeScaleCache = TimeScaleManager.Get.GlobalTimeScale;
            TimeScaleManager.Get.ChangeGlobalTimeScale(0f);
            
            if (m_DataProvider.GetPassageCount() <= 0)
            {
                OnPassageSelected(-1);
                return;
            }
            
            var cameraController = FindObjectOfType<CameraController>();
            Debug.Assert(cameraController!= null, $"{nameof(CameraController)} is not found in the scene.");
            cameraController.ResetOrientation();

            var passageSelectUI = FindObjectOfType<PassageSelectUI>();
            Debug.Assert(passageSelectUI != null, $"{nameof(PassageSelectUI)} is not found in the scene.");
            passageSelectUI.Show(m_DataProvider, OnPassageSelected);
        }

        private void OnPassageSelected(int passageIndex)
        {
            if (passageIndex >= 0)
            {
                var railPath = FindObjectOfType<RailPath>();
                Debug.Assert(railPath != null, $"{nameof(RailPath)} is not found in the scene.");
                railPath.AddPath(m_DataProvider.GetPathAtIndex(passageIndex));
            }

            var stageGenerator = FindObjectOfType<StageGenerator>();
            Debug.Assert(stageGenerator != null, $"{nameof(StageGenerator)} is not found in the scene.");
            stageGenerator.SpawnMapSector(passageIndex);
            
            TimeScaleManager.Get.ChangeGlobalTimeScale(m_GlobalTimeScaleCache);
        }

        public static void CreateFromTrigger(StageSectorEventTrigger trigger, IPassageSelectEventDataProvider provider)
        {
            var executor = trigger.gameObject.AddComponent<PassageSelectEventExecutor>();
            executor.trigger = trigger;
            executor.trigger.onEnterTrigger += executor.OnTriggerPerformed;
            executor.m_DataProvider = provider;
        }
    }

    public interface IPassageSelectEventDataProvider
    {
        public int GetPassageCount();

        public PathCreator GetPathAtIndex(int index);
    }
}