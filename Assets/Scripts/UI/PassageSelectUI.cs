using System;
using Data;
using Environments;
using InputComponents;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace UI
{
    public class PassageSelectUI : MonoBehaviour, IInputHandler
    {
        [SerializeField] private Transform selectPathDecal;
        [SerializeField] private float selecthPathDecalDistance = 30f;
        [SerializeField] private GameObject selectInformationPanel;
        //[SerializeField] private TextMeshProUGUI selectPathGuidanceText;
        //[SerializeField] private TextMeshProUGUI selectText;
        //[SerializeField] private TextMeshProUGUI selectCurrentPathText;
        //[SerializeField] private TextIdentifier selectPathGuidanceIdentifier;
        //[SerializeField] private TextIdentifier selectIdentifier;
        //[SerializeField] private TextIdentifier selectCurrentPathIdentifier;

        private CameraUtility.CameraController m_CameraController;
        
        private IPassageSelectEventDataProvider m_Provider;
        
        private readonly AxisInputIntervalModule m_AxisInputIntervalModule = new();

        private Vector2 m_LastAxisInputValue;

        private Transform m_LastTarget;
        
        private Action<int> m_OnSelectedCallback;

        private string m_selectCurrentPathTextData;

        private int CurrentSelectedPathIndex { get; set; }
        
        #region MonoBehaviour Events

        private void Awake()
        {
            enabled = false;
            selectInformationPanel.SetActive(enabled);
            selectPathDecal.gameObject.SetActive(true);
            //selectPathGuidanceText.text = DatabaseUtility.TryGetData(Database.TextData, selectPathGuidanceIdentifier.Identifier, out var selectPathGudanceTextData)
            //    ? selectPathGudanceTextData.korean : null;
            //selectText.text = DatabaseUtility.TryGetData(Database.TextData, selectIdentifier.Identifier, out var selectTextData)
            //    ? $"{selectTextData.korean} : Space / (A)" : null;
            //m_selectCurrentPathTextData = DatabaseUtility.TryGetData(Database.TextData, selectCurrentPathIdentifier.Identifier, out var selectCurrentPathTextData)
            //    ? selectCurrentPathTextData.korean : null;
        }

        private void OnGUI()
        {
            if (!enabled)
            {
                return;
            }
            
            for (var i = 0; i < m_Provider.GetPassageCount(); i += 1)
            {
                if (GUI.Button(new Rect(10, 10 + 30 * i, 100, 20), $"Passage {i + 1}"))
                {
                    HandlePassageSelected(i);
                }
            }
        }
        
        #endregion // MonoBehaviour Events
        
        #region PassageSelectUI Implementations

        public void Show(IPassageSelectEventDataProvider provider, Action<int> onSelected)
        {
            m_Provider = provider;
            m_OnSelectedCallback = onSelected;
            m_CameraController = FindObjectOfType<CameraUtility.CameraController>();
            m_LastTarget = m_CameraController?.FollowTarget;
            m_AxisInputIntervalModule.Reset();

            CurrentSelectedPathIndex = 0;
            selectPathDecal.gameObject.SetActive(false);
            
            InputComponent.Get.RegisterHandler(this);

            enabled = true;
            selectInformationPanel.SetActive(enabled);
            SelectNextPassage(CurrentSelectedPathIndex);
        }

        private void HandlePassageSelected(int index)
        {
            if (CurrentSelectedPathIndex == -1)
            {
                return;
            }
            
            m_OnSelectedCallback?.Invoke(index);
            
            InputComponent.Get.UnregisterHandler(this);
            
            enabled = false;
            selectInformationPanel.SetActive(enabled);
            selectPathDecal.gameObject.SetActive(enabled);
            m_CameraController.FollowTarget = m_LastTarget;
        }

        private void SelectNextPassage(int value)
        {
            CurrentSelectedPathIndex += value;
            if (CurrentSelectedPathIndex < 0)
            {
                CurrentSelectedPathIndex = m_Provider.GetPassageCount() - 1;
            }
            else if (CurrentSelectedPathIndex >= m_Provider.GetPassageCount())
            {
                CurrentSelectedPathIndex = 0;
            }

            var path = m_Provider.GetPathAtIndex(CurrentSelectedPathIndex);
            path.path.GetPointAndRotationAtDistance(out var point, out var rotation, path.path.length - selecthPathDecalDistance);

            //selectCurrentPathText.text = $"{m_selectCurrentPathTextData} : {(CurrentSelectedPathIndex + 1)}번 경로";
            selectPathDecal.gameObject.SetActive(true);
            selectPathDecal.SetPositionAndRotation(point, rotation);
            m_CameraController.FollowTarget = selectPathDecal;
        }

        #endregion // PassageSelectUI Implementations
        
        #region IInputHandler
        
        public void ResetInputHandlingState()
        {
        }

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context)
        {
            if (!enabled || context.phase != InputActionPhase.Performed)
            {
                return false;
            }

            var value = context.ReadValue<float>();
            SelectNextPassage(Mathf.RoundToInt(value));
            return true;
        }

        public bool HandlePlayerZoom(InputAction.CallbackContext context) => false;

        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context) => false;

        public bool HandlePlayerMoveCamera(InputAction.CallbackContext context) => false;

        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context) => true;

        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => true;

        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context) => false;

        public bool HandleUINavigate(InputAction.CallbackContext context) => false;

        public bool HandleUISubmit(InputAction.CallbackContext context)
        {
            if (!enabled || context.phase != InputActionPhase.Performed)
            {
                return false;
            }
            
            HandlePassageSelected(CurrentSelectedPathIndex);
            return true;
        }

        public bool HandleUIApply(InputAction.CallbackContext context)
        {
            return true;
        }

        public bool HandleUICancel(InputAction.CallbackContext context) => true;

        public bool HandleUIPoint(InputAction.CallbackContext context) => false;

        public bool HandleUIClick(InputAction.CallbackContext context) => false;

        public bool HandleUIRightClick(InputAction.CallbackContext context) => false;
        #endregion // IInputHandler
    }
}