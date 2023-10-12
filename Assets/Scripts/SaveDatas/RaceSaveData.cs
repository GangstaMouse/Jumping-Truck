using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RaceSaveData
{
    public string ID;
    // Изменить наверное, по умолчанию что бы значение времени было -1, опять же - наверное 
    public long BestTimeTicks;
    public int BestScore;

    public RaceSaveData(string id) => ID = id;
}
