using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public ScoreCalculator Instance { get; private set; }
    private List<CustomWheelCollider> wheels = new();

    private float flyTime;

    public float flyScore { get; private set; }

    [SerializeField] private float flyTimeScoreMultiple = 1f;
    [SerializeField] private float flyTimeThreasold = 0.2f;

    public static event Action<float> OnScoreChanged;

    private void Awake()
    {
        Instance = this;
        wheels = new List<CustomWheelCollider>(GetComponentsInChildren<CustomWheelCollider>());
    }

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

        OnScoreChanged?.Invoke(GetTotalScore());
    }

    private bool DoesCarGrounded()
    {
        foreach (var wheel in wheels)
            if (wheel.HasContact)
                return true;

        return false;
    }

    public float GetTotalScore() => flyScore;
}
