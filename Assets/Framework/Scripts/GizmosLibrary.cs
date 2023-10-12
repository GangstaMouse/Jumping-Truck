using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosLibrary
{
    // Добавить не обязательный множитель скорости вращения, и смещения, а так же расстояние между стрелками
    public static void DrawArrowedLine(Vector3 start, Vector3 end)
    {
        Gizmos.DrawLine(start, end);

        Vector3 direction = (end - start).normalized;
        float distance = (end - start).magnitude;

        Vector3 leftArrowDir = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, 30, 0) * Vector3.back * 0.15f);
        Vector3 rightArrowDir = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, -30, 0) * Vector3.back * 0.15f);

        // New
        float rotationSpeed = Time.realtimeSinceStartup * 360f;

        Vector3 leftArrowVector = Quaternion.AngleAxis(rotationSpeed, direction) * leftArrowDir;
        Vector3 rightArrowVector = Quaternion.AngleAxis(rotationSpeed, direction) * rightArrowDir;
        // ---

        // Попробовать добавить движение в перёд
        for (float i = 1; i < distance; i += 1f)
        {
            Vector3 arrowPostion = start + (direction * i);

            // Gizmos.DrawLine(arrowPostion, arrowPostion + leftArrowDir);
            // Gizmos.DrawLine(arrowPostion, arrowPostion + rightArrowDir);

            // New
            Gizmos.DrawLine(arrowPostion, arrowPostion + leftArrowVector);
            Gizmos.DrawLine(arrowPostion, arrowPostion + rightArrowVector);
            // ---
        }
    }

    public static void DrawTwoColoredArrowedLine(Vector3 start, Vector3 end, Color startColor, Color endColor, int stepsCount = 4)
    {
        DrawTwoColoredLine(start, end, startColor, endColor, stepsCount);

        Vector3 direction = (end - start).normalized;
        float distance = (end - start).magnitude;

        Vector3 leftArrowDir = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, 30, 0) * Vector3.back * 0.15f);
        Vector3 rightArrowDir = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, -30, 0) * Vector3.back * 0.15f);

        // New
        float rotationSpeed = Time.realtimeSinceStartup * 360f;

        Vector3 leftArrowVector = Quaternion.AngleAxis(rotationSpeed, direction) * leftArrowDir;
        Vector3 rightArrowVector = Quaternion.AngleAxis(rotationSpeed, direction) * rightArrowDir;
        // ---

        // Попробовать добавить движение в перёд
        for (float i = 1; i < distance; i += 1f)
        {
            Vector3 arrowPostion = start + (direction * i);

            // Gizmos.DrawLine(arrowPostion, arrowPostion + leftArrowDir);
            // Gizmos.DrawLine(arrowPostion, arrowPostion + rightArrowDir);

            Gizmos.color = Color.Lerp(startColor, endColor, i / distance);

            // New
            Gizmos.DrawLine(arrowPostion, arrowPostion + leftArrowVector);
            Gizmos.DrawLine(arrowPostion, arrowPostion + rightArrowVector);
            // ---
        }
    }

    public static void DrawTwoColoredLine(Vector3 start, Vector3 end, Color startColor, Color endColor, int stepsCount = 4)
    {
        Vector3 direction = (end - start).normalized;
        float distance = (end - start).magnitude;

        // Gizmos.DrawLine(start, start + (direction * (distance * 1)));
        // Gizmos.DrawLine(start, end);
        float segmentLenght = distance / (float)stepsCount;

        for (int i = 0; i < stepsCount; i++)
        {
            Vector3 segmentStart = start + (direction * (segmentLenght * i));
            // Vector3 segmentEnd = direction * (segmentLenght * (i + 1));
            Vector3 segmentEnd = segmentStart + (direction * segmentLenght);
            Color segmentColor = Color.Lerp(startColor, endColor, i / (stepsCount - 1f));

            Gizmos.color = segmentColor;
            Gizmos.DrawLine(segmentStart, segmentEnd);

            // Test
            /* UnityEditor.Handles.color = segmentColor;
            UnityEditor.Handles.DrawLine(segmentStart, segmentEnd, 4f); */
        }
    }

    public static void DrawCircle(Vector3 center, Vector3 up, Vector3 forward, float radius, int stepsCount = 16)
    {
        Vector3 firstDirection = Quaternion.AngleAxis(0f, up) * forward;

        for (int i = 1; i <= stepsCount; i++)
        {
            Vector3 secondDirection = Quaternion.AngleAxis((360f / stepsCount) * i, up) * forward;
            Gizmos.DrawLine(center + (firstDirection * radius), center + (secondDirection * radius));
            firstDirection = secondDirection;
        }
    }

    public static void DrawTwoColoredLineByCurve(Vector3 start, Vector3 end, AnimationCurve curve, Color startColor, Color endColor, int stepsCount = 4)
    {
        Vector3 direction = (end - start).normalized;
        float distance = (end - start).magnitude;

        float segmentLenght = distance / (float)stepsCount;

        for (int i = 0; i < stepsCount; i++)
        {
            Vector3 segmentStart = start + (direction * (segmentLenght * i));
            Vector3 segmentEnd = segmentStart + (direction * segmentLenght);
            // Color segmentColor = Color.Lerp(startColor, endColor, i / (stepsCount - 1f));
            Color segmentColor = Color.Lerp(startColor, endColor, curve.Evaluate((1f / stepsCount) * i));

            Gizmos.color = segmentColor;
            Gizmos.DrawLine(segmentStart, segmentEnd);
        }
    }

    public static void DrawArc(Vector3 center, Vector3 up, Vector3 forward, float radius, float angle, int stepsCount = 16)
    {
        float normalizedAngle = angle / 2f;
        Vector3 firstDirection = Quaternion.AngleAxis(-normalizedAngle, up) * forward;

        for (int i = 1; i <= stepsCount; i++)
        {
            float segmentAngle = (angle / stepsCount) * i;
            Vector3 secondDirection = Quaternion.AngleAxis(-normalizedAngle + segmentAngle, up) * forward;
            Gizmos.DrawLine(center + (firstDirection * radius), center + (secondDirection * radius));
            firstDirection = secondDirection;
        }
    }

    public static void DrawWirePlane(Vector3 center, Vector3 right, Vector3 up, Vector2 size)
    {
        Vector3 x = right * size.x;
        Vector3 y = up * size.y;

        Gizmos.DrawLine(center - x + y, center + x + y);
        Gizmos.DrawLine(center + x + y, center + x - y);
        Gizmos.DrawLine(center + x - y, center - x - y);
        Gizmos.DrawLine(center - x - y, center - x + y);
    }
}
