using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Network.Framework;
using Multiplayer.Runtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Steamworks;

public class NetworkController : NetworkBehaviour
{
    [SerializeField] private GameObject warningScreen;
    [SerializeField] private Button inviteButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [SerializeField] private Itemloader itemLoader;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GravityGun pickUp;

    void Start()
    {
        yesButton.onClick.AddListener(() =>
        {
            QuitServer();
        });
        noButton.onClick.AddListener(() =>
        {
            if (!IsOwner) return;

            warningScreen.SetActive(false);
        });

        if (!IsServer)
        {
            inviteButton.image.color = new Color(inviteButton.image.color.r, inviteButton.image.color.g, inviteButton.image.color.b, 0.85f);
            var textColor = inviteButton.GetComponentInChildren<TextMeshProUGUI>();
            textColor.color = new Color(textColor.color.r, textColor.color.g, textColor.color.b, 0.85f);
            inviteButton.interactable = false;
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += GoToMenu;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (!IsOwner) return;

            QuitServer();
        }
    }

    public void QuitMenu()
    {
        if (!IsOwner) return;

        warningScreen.SetActive(true);
    }

    public void QuitServer()
    {
        if (!IsOwner) return;

        if (IsServer) pickUp.DropOnQuit();
        else
        {
            int[] ids = new int[inventory.invSlots.Length];
            int[] types = new int[inventory.invSlots.Length];

            for (int i = 0; i < inventory.invSlots.Length; i++)
            {
                ids[i] = inventory.invSlots[i].id;
            }
            for (int i = 0; i < inventory.invSlots.Length; i++)
            {
                types[i] = (int)inventory.invSlots[i].type;
            }

            pickUp.DropOnQuitServerRpc(ids, inventory.invSlots.Length, types);
        }

        if (IsServer) itemLoader.Save();

        if (IsClient)
        {
            NW_NetworkManager.Instance.RequestDisconnect();
            GameNetworkManager.Instance.Disconnect();
            SceneManager.LoadScene("MenuSteam");
        }
        else
        {
            NW_NetworkManager.Instance.RequestDisconnect();
            GameNetworkManager.Instance.Disconnect();
        }
    }

    void GoToMenu(ulong id) 
    {
        SceneManager.LoadScene("MenuSteam");
    }

    public void FriendsOverlayActivate()
    {
        if (!IsServer) return;
        //ISteamFriends.ActivateGameOverlayInviteDialog(SteamClient.SteamId);
        SteamFriends.OpenOverlay("friends");
    }

}
