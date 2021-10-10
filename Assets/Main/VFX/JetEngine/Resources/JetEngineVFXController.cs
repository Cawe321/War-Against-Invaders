using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script that controls a <see cref="ParticleSystem"></see>'s Start Duration property to simulate jet engine power according to given values.
/// </summary>
public class JetEngineVFXController : MonoBehaviour
{

    [Range(0f,100f)]
    public float _percentage;

    [SerializeField]
    [Tooltip("The reference ParticleSystem component. Will automatically find one if null")]
    ParticleSystem particleSystem;

    [SerializeField]
    [Tooltip("The Start Lifetime value when the engine power is supposedly 0")]
    float minSpeedLifetime;

    [SerializeField]
    [Tooltip("The Start Lifetime value when the engine power is supposedly at its max")]
    float maxSpeedLifetime;

    void Start()
    {
        // Checks if the reference to particle system has been set
        if (particleSystem == null)
            particleSystem = transform.GetComponent<ParticleSystem>();

        if (minSpeedLifetime > maxSpeedLifetime)
        {
            float swappedValue = minSpeedLifetime;
            minSpeedLifetime = maxSpeedLifetime;
            maxSpeedLifetime = swappedValue;
        }
    }

    private void Update()
    {
        SetEnginePowerByPercentage(_percentage);
    }

    /// <summary>
    /// Sets the engine power based on the given percentage.
    /// </summary>
    /// <param name="percentage">Percentage is the strength of the engine power based on its minimum and max power. 1f is 1%</param>
    public void SetEnginePowerByPercentage(float percentage)
    {
        float calculatedDuration = (maxSpeedLifetime - minSpeedLifetime) * (percentage * 0.01f) + minSpeedLifetime;
        AssignStartLifetime(calculatedDuration);
    }

    /// <summary>
    /// Function that sets the ParticleSystem's Start Lifetime
    /// </summary>
    /// <param name="newStartLifetime"></param>
    void AssignStartLifetime(float newStartLifetime)
    {
        ParticleSystem.MainModule main = particleSystem.main;
        main.startLifetime = newStartLifetime;
    }
}
