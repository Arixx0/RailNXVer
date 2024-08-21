using HutongGames.PlayMaker.Actions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Scale_Controller : MonoBehaviour
{
    [Header("<color=orange><b> Check Ary & Resolution")]
    [Space(10)]
    [SerializeField] List<CanvasScaler> canvasScalerList = new List<CanvasScaler>();
    [Space]
    [SerializeField] Vector2 currentResolution = Vector2.zero;
    
    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
        SceneManager.sceneLoaded += CanvasCheck;
    }

    private void Update()
    {
        CheckResolutionChange();
    }

    // 1. 씬넘어갈때 
    private void CanvasCheck(Scene scene, LoadSceneMode mode)
    {
        canvasScalerList.Clear();

        CanvasScaler[] canvasScalers = FindObjectsOfType<CanvasScaler>();

        foreach (var scaler in canvasScalers)
        {
            canvasScalerList.Add(scaler);
        }

        CheckResolutionChange();
    }

    // 2. Realtime 해상도 Check
    private void CheckResolutionChange()
    {
        Vector2 newResolution = new Vector2(Screen.width, Screen.height);

        if (newResolution != currentResolution)
        {
            currentResolution = newResolution;
            UpdateCanvasScalers();
        }
    }

    // 3. 해상도 변경 
    private void UpdateCanvasScalers()
    {
        foreach (var scaler in canvasScalerList)
        {
            if (Screen.height == 720)
            {
                scaler.referenceResolution = new Vector2(1280, 720);
            }
            else if (Screen.height == 1080)
            {
                scaler.referenceResolution = new Vector2(1920, 1080);
            }
        }
    }
}
