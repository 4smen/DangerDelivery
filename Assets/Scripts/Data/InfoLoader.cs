using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Unity.Netcode;

public class InfoLoader : NetworkBehaviour
{
    [SerializeField] private GameObject handlerPrefab;

    void Awake()
    {
        if (!IsServer) return;

        var handler = Instantiate(handlerPrefab);
        LoadGeneralGameInfo(handler.GetComponent<GameInfo>()); //загружаем инфу в менеджер

        var handlerNet = handler.GetComponent<NetworkObject>();
        handlerNet.Spawn();
    }

    void LoadGeneralGameInfo(GameInfo handler)
    {
        if (File.Exists(Application.persistentDataPath
          + "/DDGI.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
              File.Open(Application.persistentDataPath
              + "/DDGI.dat", FileMode.Open);
            SaveInfo data = (SaveInfo)bf.Deserialize(file);
            handler.planDaysLeft.Value = data.savedPlanDaysLeft;
            handler.planCount.Value = data.savedPlanCount;
            handler.plan.Value = data.savedPlan;
            data.savedPlanCount = 0;
            data.savedPlanDaysLeft = 0;
            file.Close();
            Debug.Log("Item data loaded!");
        }
        else
        {
            Debug.LogError("There is no save data!");
        }
    }

    public void SaveGeneralGameInfo(GameInfo handler)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
          + "/DDGI.dat");
        SaveInfo data = new SaveInfo();
        data.savedPlanDaysLeft = handler.planDaysLeft.Value;
        data.savedPlanCount = handler.planCount.Value;
        data.savedPlan = handler.plan.Value;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Item data saved!");
    }
}

[Serializable]
class SaveInfo
{
    public int savedPlanDaysLeft;
    public int savedPlanCount;
    public int savedPlan;
}
