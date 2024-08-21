using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Manager
{
    public class SceneUtilityManager : SingletonObject<SceneUtilityManager>
    {
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(DoLoadSceneAsync(sceneName));
        }

        private IEnumerator DoLoadSceneAsync(string sceneName)
        {
            // Load designated scene when the initialize tasks complete
            Debug.Assert(
                !string.IsNullOrEmpty(sceneName) && SceneManager.GetSceneByName(sceneName) != null,
                $"{nameof(sceneName)} is not set or invalid.");
            
            var currentActiveScene = SceneManager.GetActiveScene();
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return SceneManager.UnloadSceneAsync(currentActiveScene);

            var sceneInitializer = FindObjectOfType<SceneInitializer>();
            Debug.Assert(sceneInitializer != null, $"SceneInitializer not found in {sceneName}. Some component may not work.");
            sceneInitializer?.Initialize();
        }
    }
}