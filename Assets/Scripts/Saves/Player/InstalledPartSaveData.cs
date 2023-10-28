using System;
using UnityEngine;

[Serializable]
public class InstalledPartSaveData
{
    [SerializeField] private string m_SocketID;
    [SerializeField] private string m_PartID;
    public string SocketID => m_SocketID;
    public string PartID { get => m_PartID; internal set => m_PartID = value; }

    public InstalledPartSaveData(string sockedID, string partID)
    {
        m_SocketID = sockedID;
        PartID = partID;
    }
}
