using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeSlot : BaseItemSlot
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text partText;

    public override void Initialize(object data)
    {
        base.Initialize(data);

        BasePart partData = (BasePart)this.Data;

        if (partData == null)
        {
            // nameText.SetText("Empty");
            nameText.SetText("None");
            return;
        }

        nameText.SetText(partData.name);
        // partText.SetText(socket.InstalledPart != null ? socket.InstalledPart.name : "Empty");

        if (string.IsNullOrEmpty(partData.UnlockRequirements))
            return;

        List<string> requirements = new(partData.UnlockRequirements.Replace(" ", null).Split(','));

        foreach (var item in requirements)
        {
            string requirement;
            string value;

            FunctionsLibrary.GetValuesFromCommand(item, out requirement, out value);

            if (requirement == "level")
            {
                partText.SetText($"{requirement} {value}");
            }
        }
    }
}
