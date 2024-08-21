using System;
using UnityEngine;
using Utility;

public class TimeScaleManager : SingletonObject<TimeScaleManager>
{
    [SerializeField]
    private TimeScaleTable timeScaleTable = new();
    
    public float GlobalTimeScale => timeScaleTable[TimeScaleTable.GlobalTimeScaleTag].TimeScale;

#if UNITY_EDITOR
    private void Reset()
    {
        timeScaleTable.Add(TimeScaleTable.GlobalTimeScaleTag, new TimeScaleContext(1f));
        
        var tagsInProject = UnityEditorInternal.InternalEditorUtility.tags;
        foreach (var e in tagsInProject)
        {
            timeScaleTable.TryAdd(e, new TimeScaleContext(1f));
        }

        timeScaleTable.EnsureCapacity(timeScaleTable.Count);
    }
#endif

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        foreach (var context in timeScaleTable)
        {
            context.Value.Update(deltaTime);
        }
    }

    public float GetTimeScaleOfTag(string tag)
    {
        var value = timeScaleTable[TimeScaleTable.GlobalTimeScaleTag].TimeScale;
        if (timeScaleTable.TryGetValue(tag, out var localTimeScale))
        {
            value *= localTimeScale.TimeScale;
        }

        return value;
    }

    public float GetDeltaTimeOfTag(string tag)
    {
        var timeScale =
            timeScaleTable[TimeScaleTable.GlobalTimeScaleTag].TimeScale *
            (timeScaleTable.TryGetValue(tag, out var localTimeScale) ? localTimeScale.TimeScale : 1f);
        
        return Time.unscaledDeltaTime * timeScale;
    }

    public void ChangeGlobalTimeScale(float timeScale)
    {
        timeScaleTable[TimeScaleTable.GlobalTimeScaleTag].ChangeTimeScale(timeScale);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/Train Craft/Time Scale Manager")]
    private static void CreateTimeScaleManager()
    {
        if (FindObjectOfType<TimeScaleManager>() != null)
        {
            return;
        }

        var timeScaleManagerObject = new GameObject("TimeScaleManager");
        timeScaleManagerObject.AddComponent<TimeScaleManager>();
    }
#endif

    [Serializable]
    public class TimeScaleTable : SerializableDictionary<string, TimeScaleContext>
    {
        public const string GlobalTimeScaleTag = "Global";
    }

    [Serializable]
    public class TimeScaleContext
    {
        [SerializeField, Range(0f, 100f)]
        private float timeScale;

        [SerializeField]
        private float actualTimeScale;

        [SerializeField]
        private float time;

        private bool m_IsTimeScaleTransitioning;

        public float TimeScale
        {
            get => actualTimeScale;
            set
            {
                timeScale = value;
                actualTimeScale = value;
                m_IsTimeScaleTransitioning = false;
            }
        }

        public TimeScaleContext(float timeScale)
        {
            this.timeScale = timeScale;
            this.actualTimeScale = timeScale;
            m_IsTimeScaleTransitioning = false;
        }

        public void Awake()
        {
            time = 0;
            actualTimeScale = timeScale;
        }

        public void Update(float deltaTime)
        {
            time += deltaTime * actualTimeScale;
            
            if (!m_IsTimeScaleTransitioning)
            {
                return;
            }
            
            actualTimeScale = Mathf.Lerp(actualTimeScale, timeScale, deltaTime * 5.0f);

            if (Mathf.Abs(timeScale - actualTimeScale) < 0.01f)
            {
                actualTimeScale = timeScale;
                m_IsTimeScaleTransitioning = false;
            }
        }

        public void ChangeTimeScale(float targetTimeScale)
        {
            timeScale = targetTimeScale;
            m_IsTimeScaleTransitioning = true;
        }
    }
}