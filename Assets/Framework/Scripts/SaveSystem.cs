using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string FolderPath => Application.persistentDataPath;

    public static void SaveFile(string fileName, object saveData)
    {
        string filePath = MakePath(fileName);

        BinaryFormatter formatter = new();
        FileStream stream = File.Create(filePath);

        string json = JsonUtility.ToJson(saveData, true);

        formatter.Serialize(stream, json);
        stream.Close();

        Debug.Log($"Save file <{fileName}> saved");
    }

    public static T LoadFile<T>(string fileName) where T : class
    {
        string filePath = MakePath(fileName);

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new();
            FileStream stream = File.Open(filePath, FileMode.Open);

            string json = (string)formatter.Deserialize(stream);

            var saveData = JsonUtility.FromJson<T>(json);
            stream.Close();

            Debug.Log($"Save file <{fileName}> loaded");
            return saveData;
        }

        Debug.Log($"Save file <{fileName}> not loaded");
        return null;
    }

    public static void DeleteFile(string fileName)
    {
        string filePath = MakePath(fileName);

        Debug.Log($"Trying delete save file <{fileName}> at path {FolderPath}");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save file <{fileName}> successfully deleted");
        }
    }

    public static bool DoesSaveFileExists(string fileName)
    {
        string filePath = MakePath(fileName);
        bool exists = File.Exists(filePath);
        Debug.Log($"Save data <{fileName}> {(exists ? "exists" : "not exists")}");
        return exists;
    }

    private static string MakePath(string fileName) => $"{FolderPath}/{fileName}.txt";
}
