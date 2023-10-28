using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class OptionsSaveData
{
    #region Graphics
    [SerializeField] private int m_FrameRateLimit = 30;
    [SerializeField] private int m_Quality = 1;
    public int FrameRateLimit { get => m_FrameRateLimit; set => m_FrameRateLimit = math.max(30, value); }
    public int Quality { get => m_Quality; set => m_Quality = math.max(0, value); }

    #region Post Processing
    public bool ColorCorrection = true;
    public bool Vignette = false;
    public bool Bloom = false;
    #endregion
    #endregion

    #region Controls
    public int PedalsType;
    public int SteeringType;
    #endregion
}
