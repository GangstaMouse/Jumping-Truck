using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePart : ScriptableObject
{
    [Header("Base Data")]
    public string Name;
    public string Description;
    public CollectionType collection;
    public bool Hiden;
    public string Condition;
    public float Cost;
    public int MinLevel;
    // public int MinReputation;
    // Или если будет выполнено какое то определенное задание / событие

    public virtual void InstallPart(PartSocket newPartSocket, CarController newVehicle)
    {
        
    }
}

public enum CollectionType {Buy, Collectible}