using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Shield : MonoBehaviour
{
    [Header("<color=orange><b># Setting Shield Value")]
    [Space(10)]
    [SerializeField, Range(0f, 3f)] private float m_Popup_Duration = 1f;
    [SerializeField] private bool isShieldActive = false;
    [SerializeField] private bool EffectTestMode = false;

    [Space(10)]
    [Header("<color=orange><b># Notice")]
    [SerializeField, Multiline] private string Notice;

    private ParticleSystem[] defaultShieldPs;
    private ParticleSystem onHitPs;
    private List<Material> matAry = new List<Material>();

    private bool doInit = false;
    private Coroutine effectCou;
    private float changeTimer = 0f;

    #region Initialization

    private void Awake()
    {
        VFX_Init();
    }

    private void VFX_Init()
    {
        defaultShieldPs = new ParticleSystem[2];
        defaultShieldPs[0] = transform.GetChild(0).GetComponent<ParticleSystem>();
        defaultShieldPs[1] = defaultShieldPs[0].transform.GetChild(0).GetComponent<ParticleSystem>();
        onHitPs = transform.GetChild(1).GetComponent<ParticleSystem>();

        foreach (ParticleSystem ps in defaultShieldPs)
        {
            Material mat = new Material(ps.GetComponent<Renderer>().material);
            mat.SetFloat("_Dissolve_Control", 0f);
            matAry.Add(mat);
            ps.GetComponent<Renderer>().material = mat;  // Assign the new material
        }

        doInit = true;
    }

    #endregion

#if UNITY_EDITOR // Effect Test Mode: 1, 2, 3 Keys

    private void Update()
    {
        if (EffectTestMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActiveShield(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActiveShield(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                onHitPs.Play();
            }
        }
    }

#endif

    /// <summary>
    /// Activates or deactivates the shield VFX.
    /// </summary>
    /// <param name="value">True to activate, false to deactivate.</param>
    public void ActiveShield(bool isActive)
    {
        if (!doInit)
        {
            VFX_Init();
        }

        if (effectCou != null)
        {
            StopCoroutine(effectCou);
        }

        effectCou = StartCoroutine(EffectActive(isActive));
    }


    private IEnumerator EffectActive(bool isActive)
    {
        float startValue = isActive ? 0f : 1f;
        float endValue = isActive ? 1f : 0f;

        for (changeTimer = 0f; changeTimer < m_Popup_Duration; changeTimer += Time.deltaTime)
        {
            float dissolveValue = Mathf.Lerp(startValue, endValue, changeTimer / m_Popup_Duration);

            foreach (Material mat in matAry)
            {
                mat.SetFloat("_Dissolve_Control", dissolveValue);
            }

            yield return null;
        }

        foreach (Material mat in matAry)
        {
            mat.SetFloat("_Dissolve_Control", endValue);
        }

        isShieldActive = isActive;
        effectCou = null;
    }

    /// <summary>
    /// Play shield on-hit effect.
    /// </summary>
    public void OnHitShield()
    {
        if (!isShieldActive)
        {
            return;
        }

        onHitPs.Play();
    }
}
