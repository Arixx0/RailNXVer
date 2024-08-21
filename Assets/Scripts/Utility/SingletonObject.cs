using UnityEngine;

namespace Utility
{
    public class SingletonObject<TComponent> : MonoBehaviour where TComponent : SingletonObject<TComponent>
    {
        private static TComponent globalInstance;

        public bool destroyOnLoad = false;

        public static TComponent Get => globalInstance;

        protected virtual void Awake()
        {
            if (!ReferenceEquals(globalInstance, null) && globalInstance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (ReferenceEquals(globalInstance, null))
            {
                globalInstance = this as TComponent;

                if (!destroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }
}