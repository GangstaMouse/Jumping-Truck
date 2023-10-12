using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float refreshLatency = 0.2f;

    private float current = 0f;
    private float average = 0f;
    private float maximum = 0f;
    private float minimum = Mathf.Infinity;

    private Display display;

    private float time;
    private int framesPassed;
    private float totalFrameRate;

    // private Vector2 labelPosition => new Vector2(10, 5);
    // private Vector2 labelSize => new Vector2(100, 20);

    private void Awake()
    {
        if (!Debug.isDebugBuild)
            Destroy(gameObject);
    }

    private void Update()
    {
        time += Time.unscaledDeltaTime;
        framesPassed ++;

        current = 1f / Time.unscaledDeltaTime;
        totalFrameRate += current;
        average = totalFrameRate / framesPassed;

        if (framesPassed > 10)
        {
            maximum = Mathf.Max(maximum, current);
            minimum = Mathf.Min(minimum, current);
        }

        if (time < refreshLatency)
            return;

        time -= refreshLatency;

        display.Current = current;
        display.Average = average;
        display.Maximum = maximum;
        display.Minimum = minimum;
    }

    [System.Obsolete("Broken matrix")]
    private void OnGUI()
    {
        return;
        float scale = ((Screen.width / 1145) + (Screen.height / 644)) / 2f;
        float posY = 5f;

        int targetFrameRate = Application.targetFrameRate;

        Vector3 matrixScale = new Vector3(scale, scale, 1f);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, matrixScale);
        int offset = 30;
        int initialSpace = (int)Screen.safeArea.position.x + offset;

        GUI.Label(new Rect(initialSpace + 10, posY, 100, 20), $"FPS - {display.Current.ToString("F1")} ({targetFrameRate})");
        GUI.Label(new Rect(initialSpace + 120, posY, 80, 20), $"Avg - {display.Average.ToString("F1")}");
        GUI.Label(new Rect(initialSpace + 200, posY, 80, 20), $"Max - {display.Maximum.ToString("F1")}");
        GUI.Label(new Rect(initialSpace + 280, posY, 80, 20), $"Min - {display.Minimum.ToString("F1")}");
    }

    private struct Display
    {
        public float Current;
        public float Average;
        public float Maximum;
        public float Minimum;
    }
}
