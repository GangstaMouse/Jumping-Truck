using UnityEngine; // replace by unity mathematics

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

    public static void DrawTwoColoredLine(Vector3 start, Vector3 end, Color startColor, Color endColor, int resolution = 4)
    {
        Vector3 direction = (end - start).normalized;
        float distance = (end - start).magnitude;

        float segmentLenght = distance / resolution;
        //Vector3 segmentStart = start;

        for (int i = 0; i <= resolution - 1; i++)
        {
            Vector3 segmentStart = start + (direction * (segmentLenght * i));
            // Vector3 segmentEnd = direction * (segmentLenght * (i + 1));
            Vector3 segmentEnd = segmentStart + (direction * segmentLenght);
            Color segmentColor = Color.Lerp(startColor, endColor, i / (float)resolution);

            Gizmos.color = segmentColor;
            Gizmos.DrawLine(segmentStart, segmentEnd);
            //segmentStart = segmentEnd;
        }
    }

    public static void DrawWiredCircle(Vector3 center, Vector3 up, Vector3 forward, float radius, int stepsCount = 16)
    {
        Vector3 start = center + (radius * (Quaternion.AngleAxis(0f, up) * forward));
        float segmentAngle = 360.0f / stepsCount;

        for (int i = 1; i <= stepsCount; i++)
        {
            Vector3 end = center + (radius * (Quaternion.AngleAxis(segmentAngle * i, up) * forward));
            Gizmos.DrawLine(start, end);
            start = end;
        }
    }

    public static void DrawWiredCylinder(Vector3 center, Vector3 up, Vector3 forward, float radius, float height, int stepsCount = 16)
    {
        Vector3 start = center + (radius * (Quaternion.AngleAxis(0f, up) * forward));
        float segmentAngle = 360.0f / stepsCount;

        float halfHeight = height / 2.0f;

        for (int i = 1; i <= stepsCount; i++)
        {
            Vector3 end = center + (radius * (Quaternion.AngleAxis(segmentAngle * i, up) * forward));

            // Top cap
            Gizmos.DrawLine(start + (up * halfHeight), end + (up * halfHeight));
            // Middle lines
            Gizmos.DrawLine(start - (up * halfHeight), start + (up * halfHeight));

            // Bottom cap
            Gizmos.DrawLine(start - (up * halfHeight), end - (up * halfHeight));

            start = end;
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

    public static void DrawAngelArc(Vector3 origin, Vector3 rotationAxis, Vector3 startPointDirection, float radius, float angle, float angleOffset = 0, int resolution = 16)
    {
        Vector3 start = origin + (radius * (Quaternion.AngleAxis(angleOffset, rotationAxis) * startPointDirection));

        float segmentAngel = angle / (float)resolution;

        for (int i = 1; i <= resolution; i++)
        {
            Vector3 end = origin + (radius * (Quaternion.AngleAxis((segmentAngel * i) + angleOffset, rotationAxis) * startPointDirection));
            Gizmos.DrawLine(start, end);
            start = end;
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

    public static void DrawThreePointsCurvedLine(Vector3 pointA, Vector3 pointB, Vector3 pointC, int stepsCount = 16)
    {
        float stepSegmentSize = 1f / stepsCount;
        Vector3 prevVector = pointA;

        for (int i = 1; i <= stepsCount; i++)
        {
            float val = stepSegmentSize * i;

            Vector3 segmentA = Vector3.Lerp(pointA, pointB, val);
            Vector3 segmentB = Vector3.Lerp(pointB, pointC, val);
            Vector3 newVector = Vector3.Lerp(segmentA, segmentB, val);

            Gizmos.DrawLine(prevVector, newVector);
            prevVector = newVector;
        }
    }
}
