using System;

[Serializable]
public class CarPartSaveData
{
    public readonly string Identifier;

    public CarPartSaveData(string id) => Identifier = id;
}
