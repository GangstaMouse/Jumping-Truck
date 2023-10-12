using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OptionsSaveData
{
    public int FramerateLimit = 0;
    public int Quality = 1;

    // Post Processing
    public bool ColorCorrection = true;
    public bool Vignette = false;
    public bool Bloom = false;

    // Controls
    public int PedalsType;
    public int SteeringType;
}
