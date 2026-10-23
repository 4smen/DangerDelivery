using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public class OneAndOnlySpawner : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPos;
    [SerializeField] private GameObject[] prefabs;

    //public override void OnNetworkSpawn()
    //{
    //    ObjectsSpawn();
    //}


    public void ObjectsSpawn(List<string> objNames)
    {
        for (int i = 0; i < spawnPos.Length; i++)
        {
            if (objNames.Contains(spawnPos[i].name)) return;
            else
            {
                var obj = Instantiate(prefabs[i], spawnPos[i].position, spawnPos[i].rotation);
                var netObj = obj.GetComponent<NetworkObject>();
                netObj.Spawn();
            }
        }
    }
}
