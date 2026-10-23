using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject loading;

    private void Start()
    {
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    private void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        loading.SetActive(true);
    }
}
