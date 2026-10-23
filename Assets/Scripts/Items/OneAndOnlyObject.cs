using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class OneAndOnlyObject : MonoBehaviour
{
    void OnMouseOver()
    {
        if (Input.GetKeyDown(Controls.instance.collectKey))
        {
            if (File.Exists(Application.persistentDataPath
          + "/DDGeneralII.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file =
                  File.Open(Application.persistentDataPath
                  + "/DDGeneralII.dat", FileMode.Open);
                SaveItems data = (SaveItems)bf.Deserialize(file);
                if (data.savesOneAndOnlyNames.Contains(gameObject.name + "SpawnPos")) return;
                var OAOList = data.savesOneAndOnlyNames;
                OAOList.Add(gameObject.name + "SpawnPos");
                data.savesOneAndOnlyNames = OAOList;
                Debug.Log(data.savesOneAndOnlyNames.Count);
                file.Close();
                Debug.Log("OAOObject data saved!");
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath
                  + "/DDGeneralII.dat");
                SaveItems data = new SaveItems();
                var OAOList = new List<string>(data.savesOneAndOnlyNames);
                OAOList.Add(gameObject.name + "SpawnPos");
                data.savesOneAndOnlyNames = OAOList;
                Debug.Log(data.savesOneAndOnlyNames.Count);
                bf.Serialize(file, data);
                file.Close();
                Debug.Log("OAOObject data saved!");
            }
        }
    }
}
