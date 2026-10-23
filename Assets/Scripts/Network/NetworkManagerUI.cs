using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Multiplayer.Runtime;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject loadingScreen;
    //[SerializeField] private Button clientButton;

    //private void Start()
    //{
    //    NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    //}

    //private void OnDestroy()
    //{
    //    if (NetworkManager.Singleton)
    //    {
    //        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
    //    }
    //}

    void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            menu.SetActive(false);
            loadingScreen.SetActive(true);
            GameNetworkManager.Instance.StartHost(5);
        });
    }
    //private void HandleServerStarted()
    //{
    //    if (NetworkManager.Singleton.IsHost)
    //    {
    //        var status = NetworkManager.Singleton.SceneManager.LoadScene("Save1", LoadSceneMode.Single);

    //        if (status != SceneEventProgressStatus.Started)
    //        {
    //            Debug.LogWarning($"Failed to load Save1 with a {nameof(SceneEventProgressStatus)}: {status}");
    //        }
    //    }
    //}
}
