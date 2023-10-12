using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarPartSaveData
{
    // При текущем функционале можно заменить на список string, кажется
    public string ID;

    public CarPartSaveData(string id) => ID = id;
}
