using Data;
using UnityEditor;
using UnityEngine;

namespace Manager
{
    public static class EditorSceneUtility
    {
        // [InitializeOnLoadMethod]
        // private static void SpawnAllRequiredObjects()
        // {
        //     // spawn application manager
        //     if (Object.FindObjectOfType<ApplicationInitializer>(true) == null)
        //     {
        //         var applicationInitializerObject = new GameObject("Application Manager");
        //         applicationInitializerObject.AddComponent<ApplicationInitializer>();
        //     }
        //     
        //     // spawn time scale manager
        //     if (Object.FindObjectOfType<TimeScaleManager>(true) == null)
        //     {
        //         var timeScaleManagerObject = new GameObject("Time Scale Manager");
        //         timeScaleManagerObject.AddComponent<TimeScaleManager>();
        //     }
        //     
        //     // spawn database loader
        //     if (Object.FindObjectOfType<DatabaseLoader>(true) == null)
        //     {
        //         var databaseLoaderObject = new GameObject("Database Loader");
        //         databaseLoaderObject.AddComponent<DatabaseLoader>();
        //     }
        // }
    }
}