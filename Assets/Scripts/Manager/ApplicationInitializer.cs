using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manager
{
    public class ApplicationInitializer : MonoBehaviour
    {
        [SerializeField]
        private string designatedSceneOnInitializeComplete;
        
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Application.targetFrameRate = 64;
            Application.runInBackground = true;
        }

        private IEnumerator Start()
        {
            SplashScreen.Begin();
            yield return new WaitUntil(() =>
            {
                SplashScreen.Draw();
                return SplashScreen.isFinished;
            });
            yield return new WaitForSeconds(1f);
            
            // Initialize Managers
            var databaseLoader = FindObjectOfType<DatabaseLoader>();
            yield return databaseLoader.Initialize();
            
            // do other stuff...

            Debug.Log("Complete Initialization, Loading designated scene...");
            SceneUtilityManager.Get.LoadSceneAsync(designatedSceneOnInitializeComplete);
        }
    }

    public interface IApplicationInitializerCallable
    {
        public IEnumerator Initialize();
    }
    
#if UNITY_EDITOR
    public class ApplicationInitializerEditor : UnityEditor.Editor
    {
        [UnityEditor.MenuItem("GameObject/Train Craft/Application Manager")]
        private static void InitializeApplication()
        {
            if (FindObjectOfType<ApplicationInitializer>() != null)
            {
                throw new System.Exception("ApplicationInitializer already exists in the scene.");
            }
            
            var applicationInitializer = new GameObject(nameof(ApplicationInitializer));
            applicationInitializer.AddComponent<ApplicationInitializer>();
            
            // const string defaultPrefabPath = "Assets/Editor Default Resources/prefab_application-initializer-default.prefab";
            //
            // var asset = (GameObject)EditorGUIUtility.Load(defaultPrefabPath);
            // if (asset == null || !asset.TryGetComponent(out ApplicationInitializer _))
            // {
            //     throw new System.Exception("Default prefab asset is missing or Prefab is not an object of type Train.");
            // }
            //
            // var trainObject = Instantiate(asset, null, true);
            // trainObject.name = nameof(ApplicationInitializer);
        }
    }
#endif
}