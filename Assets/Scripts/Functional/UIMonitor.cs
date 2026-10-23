using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class UIMonitor : NetworkBehaviour
{
    [SerializeField] private GameObject planetChoosedText;
    [SerializeField] private TextMeshProUGUI planDaysText;
    [SerializeField] private TextMeshProUGUI planText;

    void Start()
    {
        planDaysText.text = GameInfo.Singletone.planDaysLeft.Value.ToString();
        planText.text = $"{GameInfo.Singletone.planCount.Value}/{GameInfo.Singletone.plan.Value}";
    }

    [ClientRpc]
    public void PlanetChoosedClientRpc(int planetSceneId)
    {
        GameInfo.Singletone.InvokeOnPlanetChoosedCallback(planetSceneId);
    }
}
