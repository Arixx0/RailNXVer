using Environments;
using TrainScripts;
using UnityEngine;

namespace Manager
{
    public class SceneInitializer : MonoBehaviour
    {
        public StageGenerator stageGenerator;

        public Train train;
        
        public void Initialize()
        {
            stageGenerator.Initialize();
            train.Setup();
        }
    }
}