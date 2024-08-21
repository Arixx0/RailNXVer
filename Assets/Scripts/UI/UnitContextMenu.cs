using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class UnitContextMenu : MonoBehaviour
    {
        public UnitContextMenuItem menuItemPrefab;

        public List<ContextMenuItem> options;

        public Transform menuItemContainer;

        private RectTransform m_RectTransform;

        private List<UnitContextMenuItem> m_ActiveMenuItemInstances = new(8);

        private Queue<UnitContextMenuItem> m_MenuItemInstancePool = new(8);
        
        public RectTransform rectTransform
        {
            get => m_RectTransform;
            private set => m_RectTransform = value;
        }

        private void OnEnable()
        {
            m_RectTransform = (RectTransform)transform;
        }

        public void Show(Vector2 screenPosition)
        {
            rectTransform.anchoredPosition = screenPosition;
            gameObject.SetActive(true);
        }
        
        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void AddOptions(List<ContextMenuItem> newOptions)
        {
            options.AddRange(newOptions);
            
            RebuildChild();
        }

        public void ClearOptions()
        {
            options.Clear();
            
            RebuildChild();
        }

        private void RebuildChild()
        {
            foreach (var menuItemInstance in m_ActiveMenuItemInstances)
            {
                menuItemInstance.gameObject.SetActive(false);
                m_MenuItemInstancePool.Enqueue(menuItemInstance);
            }
            m_ActiveMenuItemInstances.Clear();

            foreach (var option in options)
            {
                var menuItemInstance = GetMenuItemInstance();
                menuItemInstance.text.text = option.name;
                menuItemInstance.onClick.RemoveAllListeners();
                menuItemInstance.onClick.AddListener(() => OnOptionSubmitted(option));
                
                m_ActiveMenuItemInstances.Add(menuItemInstance);
            }
        }

        private void OnOptionSubmitted(ContextMenuItem option)
        {
            Debug.Log($"{option.name} is submitted");
            option.callbacks.Invoke();
            Close();
        }
        
        private UnitContextMenuItem GetMenuItemInstance()
        {
            return m_MenuItemInstancePool.Count > 0
                ? m_MenuItemInstancePool.Dequeue()
                : Instantiate(menuItemPrefab, menuItemContainer);
        }

        [System.Serializable]
        public class ContextMenuItem
        {
            public string name;

            public UnityEvent callbacks;
        }
    }
}

