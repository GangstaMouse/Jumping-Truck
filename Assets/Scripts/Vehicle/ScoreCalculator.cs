using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    private List<WheelCollider> wheels = new List<WheelCollider>();

    private float flyTime;

    public float flyScore { get; private set; }

    [SerializeField] private float flyTimeScoreMultiple = 1f;
    [SerializeField] private float flyTimeThreasold = 0.2f;

    public event Action<float> OnScoreChanges;

    private void Awake() => wheels = new List<WheelCollider>(GetComponentsInChildren<WheelCollider>());

    private void FixedUpdate()
    {
        // Добавить проверку на касание кузова
        if (!DoesCarGrounded())
            Fly();
        else if (flyTime != 0f)
            Grounded();
    }

    private void Fly()
    {
        flyTime += Time.fixedDeltaTime;
    }

    private void Grounded()
    {
        if (flyTime >= flyTimeThreasold)
            flyScore += flyTime * (100f * flyTimeScoreMultiple);
        flyTime = 0f;

        OnScoreChanges?.Invoke(GetTotalScore());
    }

    private bool DoesCarGrounded()
    {
        foreach (var wheel in wheels)
            if (wheel.grounded)
                return true;

        return false;
    }

    public float GetTotalScore() => flyScore;
}
