using UnityEngine;

[DisallowMultipleComponent]
public class EngineData : MonoBehaviour
{
    public float MaxRPM { get; internal set; }
    public float IdleRPM { get; internal set; }
    public float CurRPM { get; internal set; }
}
