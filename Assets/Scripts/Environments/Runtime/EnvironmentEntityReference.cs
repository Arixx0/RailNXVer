using UnityEngine;

namespace Environments
{
    [CreateAssetMenu(fileName = "EnvironmentEntityReference", menuName = "TrainCraft/Environments/Environment Entity Reference")]
    public class EnvironmentEntityReference : ScriptableObject
    {
        public GameObject[] entityPrefab;
        
        public GameObject GetEntity(int index) => entityPrefab[index];
    }
}