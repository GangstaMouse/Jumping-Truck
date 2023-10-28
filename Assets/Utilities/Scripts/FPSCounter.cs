#if DEVELOPMENT_BUILD || UNITY_EDITOR
using UnityEngine;

class FPSCounter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_RefreshLatency = 0.2f;

    private float m_Current = 0f;
    private float m_Average = 0f;
    private float m_Maximum = 0f;
    private float m_Minimum = Mathf.Infinity;

    private Display m_Display;

    private float m_Time;
    private int m_FramesPassed;
    private float m_TotalFrameRate;

    private void Update()
    {
        m_Time += Time.unscaledDeltaTime;
        m_FramesPassed ++;

        m_Current = 1f / Time.unscaledDeltaTime;
        m_TotalFrameRate += m_Current;
        m_Average = m_TotalFrameRate / m_FramesPassed;

        if (m_FramesPassed > 10)
        {
            m_Maximum = Mathf.Max(m_Maximum, m_Current);
            m_Minimum = Mathf.Min(m_Minimum, m_Current);
        }

        if (m_Time < m_RefreshLatency)
            return;

        m_Time -= m_RefreshLatency;

        m_Display = new()
        {
            Current = m_Current,
            Average = m_Average,
            Maximum = m_Maximum,
            Minimum = m_Minimum,
        };
    }

    [System.Obsolete("Broken matrix")]
    private void OnGUI()
    {
        float scale = ((Screen.width / 1145) + (Screen.height / 644)) / 2f;
        float posY = 5f;

        Vector3 matrixScale = new(scale, scale, 1f);
        // GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, matrixScale);
        int offset = 30;
        int initialSpace = (int)Screen.safeArea.position.x + offset;

        GUI.Label(new Rect(initialSpace + 10, posY, 100, 20), $"FPS - {m_Display.Current.ToString("F1")} ({Application.targetFrameRate})");
        GUI.Label(new Rect(initialSpace + 120, posY, 80, 20), $"Avg - {m_Display.Average.ToString("F1")}");
        GUI.Label(new Rect(initialSpace + 200, posY, 80, 20), $"Max - {m_Display.Maximum.ToString("F1")}");
        GUI.Label(new Rect(initialSpace + 280, posY, 80, 20), $"Min - {m_Display.Minimum.ToString("F1")}");
    }

    struct Display
    {
        public float Current;
        public float Average;
        public float Maximum;
        public float Minimum;
    }
}
#endif
