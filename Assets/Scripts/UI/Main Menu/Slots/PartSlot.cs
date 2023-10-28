using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartSlot : BaseItemSlot
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text partText;

    public override void Initialize(object data)
    {
        base.Initialize(data);

        PartSocket socket = (PartSocket)Data;

        nameText.SetText(socket.Name);
        partText.SetText(socket.InstalledPart != null ? socket.InstalledPart.Name : "Empty");
    }
}
