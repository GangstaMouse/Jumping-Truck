using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Переименовать в SaveSystem
public static class SaveGame
{
    private static string folderPath => Application.persistentDataPath;

    public static void SaveFile(string fileName, object saveData)
    {
        string filePath = MakePath(fileName);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = File.Create(filePath);

        string json = JsonUtility.ToJson(saveData, true);

        formatter.Serialize(stream, json);
        stream.Close();

        Debug.Log($"Save file <{fileName}> saved");
    }

    // Заменить fileType на ref object?
    public static object LoadFile(string fileName, System.Type saveDataType)
    {
        string filePath = MakePath(fileName);

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(filePath, FileMode.Open);

            string json = (string)formatter.Deserialize(stream);

            // JsonUtility.FromJsonOverwrite(json, saveData);
            var saveData = JsonUtility.FromJson(json, saveDataType);
            //Debug.LogWarning(saveData);
            stream.Close();

            Debug.Log($"Save file <{fileName}> loaded");
            return saveData;
        }

        Debug.Log($"Save file <{fileName}> not loaded");
        return new object();
    }

    public static void DeleteFile(string fileName)
    {
        string filePath = MakePath(fileName);

        Debug.Log($"Trying delete save file <{fileName}> at path {folderPath}");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save file <{fileName}> successfully deleted");
        }
    }

    public static bool DoesSaveFileExists(string fileName)
    {
        string filePath = MakePath(fileName);
        // заменить на проверку сузествования файла
        // return Load(filePath) != null;
        bool exists = File.Exists(filePath);
        Debug.Log($"Save data <{fileName}> {(exists ? "exists" : "not exists")}");
        return exists;
    }

    private static string MakePath(string fileName)
    {
        return $"{folderPath}/{fileName}.txt"; // Временно
    }
}
