using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SocketSlot : BaseItemSlot
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text partText;

    public override void Initialize(object data, Action<BaseItemSlot, object> action)
    {
        base.Initialize(data, action);

        CarPartSocket socket = (CarPartSocket)this.data;

        nameText.SetText(socket.Name);
        partText.SetText(socket.PartData != null ? socket.PartData.name : "Empty");
    }
}
