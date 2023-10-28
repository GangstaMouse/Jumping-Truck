using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireSmokeEffect : MonoBehaviour
{
    private CustomWheelCollider wheel;
    private ParticleSystem particle;

    private void Awake()
    {
        wheel = GetComponentInParent<CustomWheelCollider>();
        particle = GetComponent<ParticleSystem>();

        ParticleSystem.MainModule main = particle.main;
        main.startColor = new Color(1f, 1f, 1f, 0f);
    }

    private void FixedUpdate()
    {
        Refresh();
    }

    private void Refresh()
    {
        Vector2 mag = wheel.CachedTireFrictionForce;

        // Полагаю тут есть проблема с производительностью из за постояных вызовов Play/Pause
        if (mag.magnitude > 0.7f)
        {
            transform.localPosition = -wheel.transform.up * (wheel.CurrentSuspensionLenght + wheel.Radius);
            particle.Play();
            ParticleSystem.MainModule main = particle.main;
            Color color = main.startColor.color;
            color.a = mag.magnitude;
            main.startColor = color;
        }
        else
        {
            particle.Stop();
        }
    }

    /* private void OnEnable() => wheel.OnPostPhysics += Refresh;

    private void OnDisable() => wheel.OnPostPhysics -= Refresh; */
}
