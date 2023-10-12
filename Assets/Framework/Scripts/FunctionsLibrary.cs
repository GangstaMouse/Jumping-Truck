using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FunctionsLibrary
{
    public static void GetValuesFromCommand(string command, out string commandName, out string commandData)
    {
        List<string> substring = new List<string>(command.Split('_'));

        commandName = substring[0];
        commandData = substring[1];
    }

    public static List<Material> GetListOfMaterialsByName(GameObject gameObject, string materialName)
    {
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>(gameObject.GetComponentsInChildren<MeshRenderer>());

        List<Material> materials = new List<Material>();

        foreach (var renderer in meshRenderers)
            foreach (var material in renderer.materials)
                if (Regex.IsMatch(material.name, materialName))
                    materials.Add(material);
        
        return materials;
    }

    public static Color GetMaterialsMostColor(List<Material> materials)
    {
        Dictionary<Color, int> colors = new Dictionary<Color, int>();

        foreach (var material in materials)
        {
            if (colors.ContainsKey(material.color))
                colors[material.color] ++;
            else
                colors.Add(material.color, 1);
        }

        Color color = Color.black;
        int number = 0;

        foreach (var item in colors)
        {
            if (item.Value > number)
            {
                color = item.Key;
                number = item.Value;
            }
        }

        return color;
    }

    // Неверное наименование ведь тут нет крайнего ограничения
    public static float MapRangeClamped(float value, float InRangeA, float InRangeB, float OutRangeA, float OutRangeB)
    {
        return OutRangeA + (value - InRangeA) * (OutRangeB - OutRangeA) / (InRangeB - InRangeA);
    }
}
