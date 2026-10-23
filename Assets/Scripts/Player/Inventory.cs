using UnityEngine;
using static Enums;
using UnityEngine.UI;
using Unity.Netcode;
using Network.Framework;
using Multiplayer.Runtime;
using Steamworks;

public class Inventory : NetworkBehaviour
{
    [SerializeField] public Item[] invSlots;
    [SerializeField] public Image[] slots;

    [SerializeField] private Sprite unselectedSlot;
    [SerializeField] private Sprite selectedSlot;

    [SerializeField] private GravityGun pickUp;

    [SerializeField] public bool locked = false;

    int scroll1;

    void Start()
    {
        NetworkManager.OnClientConnectedCallback += UpdateInfo;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            if (locked == false)
            {
                if (!IsOwner) return;

                scroll1++;

                var scroll = (int)Mathf.Repeat(scroll1, invSlots.Length);

                Debug.Log("scroll = " + scroll);

                pickUp.CurretItemUpdate(invSlots[scroll]);
                //if (IsServer) pickUp.CurretItemUpdateClientRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);
                //else pickUp.CurretItemUpdateServerRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);

                for (int i = 0; i < invSlots.Length; i++)
                {
                    if (i == scroll)
                    {
                        slots[i].sprite = selectedSlot;
                        Debug.Log("обновили в хорошую сторону " + slots[i]);
                    }
                    else
                    {
                        slots[i].sprite = unselectedSlot;
                        Debug.Log("обновили в плохую сторону " + slots[i]);
                    }
                }
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (locked == false)
            {
                if (!IsOwner) return;

                scroll1--;

                var scroll = (int)Mathf.Repeat(scroll1, invSlots.Length);

                Debug.Log("scroll = " + scroll);

                pickUp.CurretItemUpdate(invSlots[scroll]);
                //if (IsServer) pickUp.CurretItemUpdateClientRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);
                //else pickUp.CurretItemUpdateServerRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);

                for (int i = 0; i < invSlots.Length; i++)
                {
                    if (i == scroll)
                    {
                        slots[i].sprite = selectedSlot;
                        Debug.Log("обновили в хорошую сторону " + slots[i]);
                    }
                    else
                    {
                        slots[i].sprite = unselectedSlot;
                        Debug.Log("обновили в плохую сторону " + slots[i]);
                    }
                }
            }         
        }
    }

    void UpdateInfo(ulong id)
    {
        if (NetworkManager.ServerClientId == id) return;
        //if (IsServer) pickUp.CurretItemUpdateClientRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);
        //else pickUp.CurretItemUpdateServerRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);
    }

    public void RoomFind(Item item)
    {
        if (locked == false)
        {
            if (!IsOwner) return;

            var scroll = (int)Mathf.Repeat(scroll1, invSlots.Length);
            if (invSlots[scroll].type == ItemType.None)
            {
                Item info = new Item();
                info.CopyItemInfo(item);
                invSlots[scroll] = info;
                Debug.Log("отксерокопировали инфу");

                pickUp.weight += item.weight;
                Debug.Log("взяли в ручки");

                var slotImage = slots[scroll].GetComponentsInChildren<Image>();
                slotImage[1].enabled = true;
                slotImage[1].sprite = invSlots[scroll].imageOnCanvas;
                Debug.Log("опа кот");

                pickUp.CurretItemUpdate(invSlots[scroll]);
                //if (IsServer) pickUp.CurretItemUpdateClientRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);
                //else pickUp.CurretItemUpdateServerRpc(pickUp.indexMaterials, pickUp.indexModel, pickUp.currentItem.type, pickUp.matLength);
            }
        }
        //else
        //{
        //    //вы не можете брать предметы так как вы несёте бандурину и блаблабла
        //}
    }

    public void ItemRemove()
    {
        if (!IsOwner) return;

        var scroll = (int)Mathf.Repeat(scroll1, invSlots.Length);

        invSlots[scroll].NullItemInfo();
        var slotImage = slots[scroll].GetComponentsInChildren<Image>();
        slotImage[1].enabled = false;
        slotImage[1].sprite = null;
    }
}
