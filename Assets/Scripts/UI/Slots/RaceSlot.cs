using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceSlot : BaseItemSlot
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text bestTimeText;

    // public override void Initialize(object data, Action<BaseItemSlot> action)
    public override void Initialize(object data, Action<BaseItemSlot, object> action)
    {
        base.Initialize(data, action);

        if (this.data is RaceData)
        {
            RaceData raceData = (RaceData)this.data;

            nameText.SetText(raceData.Text);

            string textLabel = null;
            string valueText = null;
            bool isNull = false;

            List<string> requirements = new List<string>(raceData.Goal.Replace(" ", null).Split(','));

            /* foreach (var requirement in requirements)
            {
                string condition;
                string value;

                FunctionsLibrary.GetValuesFromCommand(requirement, out condition, out value);

                switch (condition)
                {
                    case "time":
                        break;

                    case "score":
                        break;
                }
            } */

            /* switch (raceData.Goal)
            {
                default:
                    textLabel = "Best time";
                    long timeTicks = PlayerDataProcessor.GetRaceBestTime(raceData.ID);

                    if (timeTicks == 0)
                    {
                        isNull = true;
                        break;
                    }

                    TimeSpan time = TimeSpan.FromTicks(timeTicks);
                    valueText = string.Format(@"{0:mm\:ss\:ff}", time);
                    break;

                case RaceGoal.Score:
                    textLabel = "Best score";

                    int score = PlayerDataProcessor.GetRaceScore(raceData.ID);

                    if (score == 0)
                    {
                        isNull = true;
                        break;
                    }

                    valueText = score.ToString();
                    break;
            } */



            string text = (isNull ? "D/N" : valueText);
            bestTimeText.SetText($"{textLabel} - {text}");
        }
    }
}
