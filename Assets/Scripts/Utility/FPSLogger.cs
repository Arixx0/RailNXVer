using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class FPSLogger : MonoBehaviour
    {
        private const string LogFormat = "FPS(Current:{0:N0} Average(unity stats):{1:N0}({2:N0}) Lowest:{3:N0} Highest:{4:N0})";

        private const int FPSSamplingCount = 100;

        private const int FPSSamplingRate = 144;

        private readonly Queue<float> m_FPSHistory = new(FPSSamplingCount);

        private float m_FPSSummary = 0;

        private int m_FPSSamplingCount = 0;

        public float CurrentFPS { get; private set; } = 0;

        public float AverageFPS { get; private set; } = 60;
        
        public int UnityInternalFPS { get; private set; }

        public float HighestFPS { get; private set; } = 30;

        public float LowestFPS { get; private set; } = 260;

        private void OnGUI()
        {
            GUILayout.Label(string.Format(LogFormat, CurrentFPS, AverageFPS, UnityInternalFPS, LowestFPS, HighestFPS));
        }

        private void Update()
        {
            CurrentFPS = 1f / Time.deltaTime;

            if (m_FPSHistory.Count >= FPSSamplingCount)
            {
                m_FPSSummary -= m_FPSHistory.Dequeue();
            }

            if (m_FPSSamplingCount++ >= FPSSamplingRate)
            {
                m_FPSSamplingCount = 0;
                HighestFPS = 30f;
                LowestFPS = 260f;
            }
            
            m_FPSHistory.Enqueue(CurrentFPS);
            m_FPSSummary += CurrentFPS;
            
            HighestFPS = CurrentFPS > HighestFPS ? CurrentFPS : HighestFPS;
            LowestFPS = CurrentFPS < LowestFPS ? CurrentFPS : LowestFPS;
            AverageFPS = m_FPSSummary / m_FPSHistory.Count;
            UnityInternalFPS = (int)(Time.frameCount / Time.time);
        }
    }
}