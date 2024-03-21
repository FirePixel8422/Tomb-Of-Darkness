using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveAndLoadInventory
{
    public static void SaveInfo(InventorySaveLoadFunctions p)
    {
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/InvData.Tmb";

        FileStream stream = new FileStream(path, FileMode.Create);

        InventorySaveData data = new InventorySaveData(p);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("succes");
    }

    public static InventorySaveData LoadInfo()
    {
        string path = Application.persistentDataPath + "/InvData.Tmb";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            InventorySaveData data = formatter.Deserialize(stream) as InventorySaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found yet in" + path);
            return null;
        }
    }
}
