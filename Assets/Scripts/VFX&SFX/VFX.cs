using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfxEffect;

    public void PlayVFX(float startSpeed = -1)
    {
        if (!vfxEffect.isPlaying)
        {
            vfxEffect.Play();
        }
        if (startSpeed != -1)
        {
            var mainMoudule = vfxEffect.main;
            mainMoudule.startSpeed = startSpeed;
        }
    }

    public void StopVFX()
    {
        if (vfxEffect.isPlaying)
        {
            vfxEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
