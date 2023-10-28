using System;

[Serializable]
public class RaceSaveData
{
    public string Identifier;
    // Изменить наверное, по умолчанию что бы значение времени было -1, опять же - наверное 
    public long BestTimeTicks;
    public int BestScore;

    public RaceSaveData(string id) => Identifier = id;
}
