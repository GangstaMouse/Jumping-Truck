using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngle : MonoBehaviour
{
    public float Width;
    public float Lenght;
    [Range(-1f, 1f)]
    public float SteerInput = -1f;
    public float TurnRadius = 30f;

    public float Radius;

    public float LeftRadius;
    public float RightRadius;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Первый вариант
        // Radius = Width/2f + Lenght/Mathf.Sin(1f);
        Radius = Width/2f + Lenght/Mathf.Sin(SteerInput);

        // Второй вариант
        // Решение проблемы с +- простое, всегда к turnradius прибавлять (width / 2f) * -?mathf.sign(steerInput(steerValue)) и нужно помножить ещё на один знак, знак стороны колеса, localpos.x
        // Radius = Mathf.Rad2Deg * Mathf.Atan(Lenght / (TurnRadius + (Width / 2f))) * SteerInput;
        LeftRadius = Mathf.Rad2Deg * Mathf.Atan(Lenght / (TurnRadius + (Width / 2f))) * SteerInput;
        RightRadius = Mathf.Rad2Deg * Mathf.Atan(Lenght / (TurnRadius - (Width / 2f))) * SteerInput;
    }

    private void OnDrawGizmos()
    {
        GizmosLibrary.DrawCircle(transform.position + (transform.right * Radius), transform.up, transform.forward, Radius, 64);
    }
    #endif
}
