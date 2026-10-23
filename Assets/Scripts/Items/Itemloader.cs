using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static Itemloader;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Steamworks;
using UnityEditor;

[Serializable]
public class Itemloader : NetworkBehaviour
{
    [Serializable]
    public struct myVector3
    {
        public float x; public float y; public float z;

        public myVector3(float newX, float newY, float newZ)
        {
            x = newX; y = newY; z = newZ;
        }
    }

    [SerializeField] private List<myVector3> objectsPositions = new List<myVector3>();
    [SerializeField] private List<float> objectsEnergy = new List<float>();
    [SerializeField] private List<int> objectsIDs = new List<int>();
    [SerializeField] private List<string> objectsNames;
    [SerializeField] private GameObject[] prefabs;

    public void OnLevelWasLoaded(int level)
    {
        if (!IsOwner) return;
        if (!IsServer) return;

        if (level == 1)
        {
            LoadItemInfo();

            if (objectsPositions != null)
            {
                foreach (var pos in objectsPositions)
                {
                    ItemSpawn(pos.x, pos.y, pos.z);
                }
            }
        }

        var sceneOAOManager = GameObject.FindObjectOfType<OneAndOnlySpawner>();

        if (File.Exists(Application.persistentDataPath
          + "/DDII.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
            File.Open(Application.persistentDataPath
              + "/DDII.dat", FileMode.Open);
            SaveItems data = (SaveItems)bf.Deserialize(file);
            Debug.Log(data.savesOneAndOnlyNames.Count);
            objectsNames = data.savesOneAndOnlyNames;
            file.Close();
            Debug.Log("savesOneAndOnlyNames data loaded! " + objectsNames.Count);
        }

        sceneOAOManager.ObjectsSpawn(objectsNames);
    }

    void ItemSpawn(float x, float y, float z)
    {
        myVector3 pos = new myVector3(x, y, z);
        var item = Instantiate(prefabs[objectsIDs[objectsPositions.IndexOf(pos)]]);
        Debug.Log(objectsIDs[objectsPositions.IndexOf(pos)]);

        item.transform.position = new Vector3(pos.x, pos.y, pos.z);

        Item info = new Item();
        info.CopyItemInfo(item.GetComponent<ItemInfo>().item);
        info.energy = objectsEnergy[objectsPositions.IndexOf(pos)];
        item.GetComponent<ItemInfo>().item = info;

        item.GetComponent<NetworkObject>().Spawn();

        Debug.Log("заспавнили " + item.name);
    }

    public void Save()
    {
        if (!IsOwner) return;

        if (SceneManager.GetActiveScene().name != "Save1") return;

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        if (objectsPositions != null)
            objectsPositions.Clear();
        if (objectsIDs != null)
            objectsIDs.Clear();
        if (objectsEnergy != null)
            objectsEnergy.Clear();

        var objPos = new List<myVector3>();
        var objIDs = new List<int>();
        var objEnergy = new List<float>();

        foreach (var item in items)
        {
            var itemPos = new myVector3(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            objPos.Add(itemPos);
            objIDs.Add(item.GetComponent<ItemInfo>().item.id);
            objEnergy.Add(item.GetComponent<ItemInfo>().item.energy);
        }

        objectsPositions = objPos;
        objectsIDs = objIDs;
        objectsEnergy = objEnergy;

        SaveItemInfo();
    }

    void LoadItemInfo()
    {
        if (File.Exists(Application.persistentDataPath
          + "/DDII.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
              File.Open(Application.persistentDataPath
              + "/DDII.dat", FileMode.Open);
            SaveItems data = (SaveItems)bf.Deserialize(file);
            objectsPositions = data.savedObjectsPositions;
            objectsEnergy = data.savedObjectsEnergy;
            objectsIDs = data.savedObjectsIDs;
            data.savedObjectsPositions = null;
            data.savedObjectsEnergy = null;
            data.savedObjectsIDs = null;
            file.Close();
            Debug.Log("Item data loaded!");
        }
        else
        {
            Debug.LogError("There is no save data!");
        }
    }

    void SaveItemInfo()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
          + "/DDII.dat");
        SaveItems data = new SaveItems();
        data.savedObjectsPositions = objectsPositions;
        data.savedObjectsEnergy = objectsEnergy;
        data.savedObjectsIDs = objectsIDs;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Item data saved!");
    }

    public void ResetItemInfo()
    {
        if (!IsHost) return;

        if (File.Exists(Application.persistentDataPath
          + "/DDII.dat"))
        {
            File.Delete(Application.persistentDataPath
              + "/DDII.dat");
            objectsPositions = null;
            objectsEnergy = null;
            objectsIDs = null;
            Debug.Log("Data reset complete!");
        }
        else
            Debug.LogError("No save data to delete.");

        gameObject.transform.parent = null;
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        NetworkManager.SceneManager.LoadScene("Save1", LoadSceneMode.Single);
    }
}

[Serializable]
class SaveItems
{
    public List<myVector3> savedObjectsPositions = new List<myVector3>();
    public List<float> savedObjectsEnergy = new List<float>();
    public List<int> savedObjectsIDs = new List<int>();
    public List<string> savesOneAndOnlyNames = new List<string>();
}
