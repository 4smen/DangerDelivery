using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Multiplayer.Runtime;
using Steamworks;
using Network.Framework;
using Steamworks.Data;
using UnityEngine.Events;

public class SteamOverlay : MonoBehaviour
{
    private void OnEnable()
    {
        SteamFriends.OpenOverlay("friends");
        //SteamFriends.OpenGameInviteOverlay(SteamClient.SteamId);
        SteamFriends.OnGameOverlayActivated += OnGameOverlayActivated;
    }

    private void OnGameOverlayActivated(bool active)
    {
        if (active != false)
        {
            Debug.Log("Steam Overlay has been activated");
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
        }
    }
}
