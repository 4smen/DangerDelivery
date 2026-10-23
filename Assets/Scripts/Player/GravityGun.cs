using UnityEngine;
using TMPro;
using static Enums;
using Unity.Netcode;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GravityGun : NetworkBehaviour
{
    [SerializeField] private float maxGrabDistance = 10f;
    [SerializeField] private TMP_InputField distanceInput;

    [SerializeField] private Transform dropPoint;
    [SerializeField] private Transform smallDropPoint;

    //[SerializeField] private MeshFilter holdableItemFilter;
    //[SerializeField] private Renderer holdableItemRenderer;

    //[SerializeField] private MeshFilter holdableSmallItemFilter;
    //[SerializeField] private Renderer holdableSmallItemRenderer;

    [SerializeField] private GameObject[] uselessPanel;

    [SerializeField] public Item currentItem;

    [SerializeField] private GameObject[] prefabs;

    //[SerializeField] private List<Material> materials = new List<Material>();
    //[SerializeField] private List<Mesh> models = new List<Mesh>();

    Vector3 itemVector3;
    //public int matLength;

    //public Material[] itemMaterials;
    //public int[] indexMaterials;
    //public int indexModel;

    public bool holding;
    public float weight = 0f;
    bool activated;

    int clicks;
    int currentUselessPanelIndex;

    float wait = 5f;
    float waiting;

    Animator animator;
    NetworkObject netObj;
    [SerializeField] private Inventory inventory;
    //[SerializeField] private Gun gun;
    [SerializeField] private Camera cam;
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();

        netObj = GetComponent<NetworkObject>();

        waiting = wait;

        distanceInput.text = maxGrabDistance.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(Controls.instance.collectKey))
        {
            if (!IsOwner) return;

            if (holding == false)
            {
                Debug.Log("валыны нет");
                //if (gun.showed == false)
                //{

                //}
                RaycastHit hit;
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(ray, out hit, maxGrabDistance))
                {
                    if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        Debug.Log("кого-то поймали");
                        if (!hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.CompareTag("Gun"))
                        {
                            if (hit.collider.gameObject.GetComponent<ItemInfo>().grabable == true)
                            {
                                if (hit.collider.gameObject.GetComponent<ItemInfo>().item.oneAndOnly == true)
                                {
                                    if (IsServer) OneAndOnlySave(hit.collider.gameObject.name);
                                    else OneAndOnlySaveServerRpc(hit.collider.gameObject.name);
                                }

                                Debug.Log("ебать, можно подобрать");
                                inventory.RoomFind(hit.collider.gameObject.GetComponent<ItemInfo>().item);

                                if (IsServer) hit.collider.gameObject.GetComponent<NetworkObject>().Despawn(destroy: true);
                                else ItemDestroyServerRpc(hit.collider.gameObject.GetComponent<NetworkObject>());
                            }
                        }
                    }
                }
            }
        }
        if (Input.GetKeyDown(Controls.instance.interactKey))
        {
            if (!IsOwner) return;

            Debug.Log("прожата кнопка");
            if (currentItem.type == ItemType.Small)
            {
                Debug.Log("предметик маленький");
                RaycastHit hit;
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(ray, out hit, maxGrabDistance))
                {
                    Debug.Log("папали");
                    //взамодействие маленьким объектом
                }
            }
        }
        if (Input.GetKeyDown(Controls.instance.dropKey))
        {
            if (holding == true)
            {
                //if (gun.showed == false)
                //{

                //}
                if (currentItem.type != ItemType.None)
                {
                    ItemDrop();
                    if (IsServer)
                    {
                        var pref = Instantiate(prefabs[currentItem.id], itemVector3, Quaternion.identity);
                        var prefNet = pref.GetComponent<NetworkObject>();
                        prefNet.Spawn();
                        //ItemDropClientRpc(netObj);
                    }
                    //else ItemDropServerRpc(netObj, currentItem.id, itemVector3);

                    currentItem.NullItemInfo();
                }
            }
        }

        if (Input.GetKeyDown(currentItem.activateButton))
        {
            if (!IsOwner) return;

            if (currentItem.activateButton != KeyCode.None)
            {
                activated = !activated;
            }
        }
        if (Input.GetKeyDown(currentItem.useButton))
        {
            if (!IsOwner) return;

            if (activated == true && currentItem.activateButton != KeyCode.None || currentItem.activateButton == KeyCode.None)
            {
                switch (currentItem.usableType)
                {
                    case UsableItemType.None: //приколямбола
                        if (clicks < 30)
                        {
                            clicks += 1;
                        }
                        else if (clicks == 30)
                        {
                            uselessPanel[currentUselessPanelIndex].SetActive(true);
                            waiting -= Time.deltaTime;
                            if (waiting <= 0)
                            {
                                uselessPanel[currentUselessPanelIndex].SetActive(false);
                                currentUselessPanelIndex += 1;
                                clicks = 0;
                                waiting = wait;
                            }
                        }
                        break;
                }
            }
        }
    }

    public void GrabDistanceChanged()
    {
        float inputedDistance = System.Convert.ToSingle(distanceInput.text);
        maxGrabDistance = inputedDistance;
    }


    [ServerRpc]
    public void ItemDestroyServerRpc(NetworkObjectReference item)
    {
        if (!item.TryGet(out NetworkObject netObj)) return;
        netObj.Despawn(destroy: true);
    }



    public void CurretItemUpdate(Item item)
    {
        if (!IsOwner) return;

        if (item.type == ItemType.None)
        {
            Debug.Log("тута пуста");
            animator.SetInteger("holdingItemType", 0);

            //holdableItemFilter.mesh = null;
            //holdableSmallItemFilter.mesh = null;
            //for (int i = 0; i < holdableItemRenderer.materials.Length; i++)
            //{
            //    holdableItemRenderer.materials[i] = null;
            //}
            //for (int i = 0; i < holdableSmallItemRenderer.materials.Length; i++)
            //{
            //    holdableSmallItemRenderer.materials[i] = null;
            //}

            holding = false;

            currentItem.NullItemInfo();

            return;
        }
        Debug.Log("ебать чёто есть");

        holding = true;

        Item info = new Item();
        info.CopyItemInfo(item);
        currentItem = info;

        //matLength = currentItem.materials.Length;

        audioSource.PlayOneShot(currentItem.pick);

        //switch (currentItem.type)
        //{
        //    case ItemType.Small:
        //        holdableSmallItemRenderer.materials = currentItem.materials;
        //        holdableSmallItemFilter.mesh = currentItem.model;
        //        break;
        //    case ItemType.Medium:
        //        holdableItemRenderer.materials = currentItem.materials;
        //        holdableItemFilter.mesh = currentItem.model;
        //        break;
        //    case ItemType.Big:
        //        //inventory.locked = true;
        //        goto case ItemType.Medium;
        //}

        //indexMaterials = null;
        //itemMaterials = null;
        //indexModel = 0;

        //var indMat = new int[currentItem.materials.Length];

        //for (int i = 0; i < currentItem.materials.Length; i++)
        //{
        //    indMat[i] = materials.IndexOf(currentItem.materials[i]);
        //}
        //indexMaterials = indMat;
        //indexModel = models.IndexOf(currentItem.model);

        animator.SetInteger("holdingItemType", (int)currentItem.type);
    }

    //[ServerRpc]
    //public void CurretItemUpdateServerRpc(int[] indexMat, int indexModel, ItemType type, int matLength)
    //{
    //    if (type == ItemType.None)
    //    {
    //        holdableItemFilter.mesh = null;
    //        holdableSmallItemFilter.mesh = null;
    //        for (int i = 0; i < holdableItemRenderer.materials.Length; i++)
    //        {
    //            holdableItemRenderer.materials[i] = null;
    //        }
    //        for (int i = 0; i < holdableSmallItemRenderer.materials.Length; i++)
    //        {
    //            holdableSmallItemRenderer.materials[i] = null;
    //        }

    //        return;
    //    }

    //    var itemMat = new Material[matLength];

    //    for (int i = 0; i < indexMat.Length; i++)
    //    {
    //        itemMat[i] = materials[indexMat[i]];
    //    }

    //    itemMaterials = itemMat;

    //    switch (type)
    //    {
    //        case ItemType.Small:
    //            holdableSmallItemRenderer.materials = itemMaterials;
    //            holdableSmallItemFilter.mesh = models[indexModel];
    //            break;
    //        case ItemType.Medium:
    //            holdableItemRenderer.materials = itemMaterials;
    //            holdableItemFilter.mesh = models[indexModel];
    //            break;
    //        case ItemType.Big:
    //            //inventory.locked = true;
    //            goto case ItemType.Medium;
    //    }
    //    CurretItemUpdateClientRpc(indexMat, indexModel, type, matLength);
    //}

    //[ClientRpc]
    //public void CurretItemUpdateClientRpc(int[] indexMat, int indexModel, ItemType type, int matLength)
    //{
    //    if (type == ItemType.None)
    //    {
    //        holdableItemFilter.mesh = null;
    //        holdableSmallItemFilter.mesh = null;
    //        for (int i = 0; i < holdableItemRenderer.materials.Length; i++)
    //        {
    //            holdableItemRenderer.materials[i] = null;
    //        }
    //        for (int i = 0; i < holdableSmallItemRenderer.materials.Length; i++)
    //        {
    //            holdableSmallItemRenderer.materials[i] = null;
    //        }

    //        return;
    //    }

    //    var itemMat = new Material[matLength];

    //    for (int i = 0; i < indexMat.Length; i++)
    //    {
    //        itemMat[i] = materials[indexMat[i]];
    //    }

    //    itemMaterials = itemMat;

    //    switch (type)
    //    {
    //        case ItemType.Small:
    //            holdableSmallItemRenderer.materials = itemMaterials;
    //            holdableSmallItemFilter.mesh = models[indexModel];
    //            break;
    //        case ItemType.Medium:
    //            holdableItemRenderer.materials = itemMaterials;
    //            holdableItemFilter.mesh = models[indexModel];
    //            break;
    //        case ItemType.Big:
    //            //inventory.locked = true;
    //            goto case ItemType.Medium;
    //    }
    //}



    public void ItemDrop()
    {
        if (!IsOwner) return;

        holding = false;

        //holdableItemFilter.mesh = null;
        //holdableSmallItemFilter.mesh = null;
        //for (int i = 0; i < holdableItemRenderer.materials.Length; i++)
        //{
        //    holdableItemRenderer.materials[i] = null;
        //}
        //for (int i = 0; i < holdableSmallItemRenderer.materials.Length; i++)
        //{
        //    holdableSmallItemRenderer.materials[i] = null;
        //}

        weight -= currentItem.weight;

        animator.SetInteger("holdingItemType", 0);
        audioSource.PlayOneShot(currentItem.drop);

        switch (currentItem.type)
        {
            case ItemType.Medium:
                itemVector3 = dropPoint.position;
                break;
            case ItemType.Big:
                //inventory.locked = false;
                goto case ItemType.Medium;
            case ItemType.Small:
                itemVector3 = smallDropPoint.position;
                break;
        }
        
        inventory.ItemRemove();
        Debug.Log("обнулили лошка");
    }

    //[ServerRpc]
    //public void ItemDropServerRpc(NetworkObjectReference player, int curItemId, Vector3 vec3)
    //{
    //    if (!player.TryGet(out NetworkObject netObjPlayer)) return;
    //    var pickUp = netObjPlayer.GetComponent<GravityGun>();

    //    pickUp.holdableItemFilter.mesh = null;
    //    pickUp.holdableSmallItemFilter.mesh = null;
    //    for (int i = 0; i < pickUp.holdableItemRenderer.materials.Length; i++)
    //    {
    //        pickUp.holdableItemRenderer.materials[i] = null;
    //    }
    //    for (int i = 0; i < pickUp.holdableSmallItemRenderer.materials.Length; i++)
    //    {
    //        pickUp.holdableSmallItemRenderer.materials[i] = null;
    //    }

    //    var pref = Instantiate(prefabs[curItemId], vec3, Quaternion.identity);
    //    var prefNet = pref.GetComponent<NetworkObject>();
    //    prefNet.Spawn();
    //}

    //[ClientRpc]
    //public void ItemDropClientRpc(NetworkObjectReference player)
    //{
    //    if (!player.TryGet(out NetworkObject netObjPlayer)) return;
    //    var pickUp = netObjPlayer.GetComponent<GravityGun>();

    //    pickUp.holdableItemFilter.mesh = null;
    //    pickUp.holdableSmallItemFilter.mesh = null;
    //    for (int i = 0; i < pickUp.holdableItemRenderer.materials.Length; i++)
    //    {
    //        pickUp.holdableItemRenderer.materials[i] = null;
    //    }
    //    for (int i = 0; i < pickUp.holdableSmallItemRenderer.materials.Length; i++)
    //    {
    //        pickUp.holdableSmallItemRenderer.materials[i] = null;
    //    }
    //}



    public void DropOnQuit()
    {
        for (int i = 0; i < inventory.invSlots.Length; i++)
        {
            if (inventory.invSlots[i].type == ItemType.None) continue;

            float randX = Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f);
            float randZ = Random.Range(transform.position.z - 0.5f, transform.position.z + 0.5f);

            var pref = Instantiate(prefabs[inventory.invSlots[i].id], new Vector3(randX, transform.position.y, randZ), Quaternion.identity);
            var prefNet = pref.GetComponent<NetworkObject>();
            prefNet.Spawn();
        }
    }

    [ServerRpc]
    public void DropOnQuitServerRpc(int[] ids, int length, int[] types)
    {
        for (int i = 0; i < length; i++)
        {
            if ((ItemType)types[i] == ItemType.None) continue;

            float randX = Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f);
            float randZ = Random.Range(transform.position.z - 0.5f, transform.position.z + 0.5f);

            var pref = Instantiate(prefabs[ids[i]], new Vector3(randX, transform.position.y, randZ), Quaternion.identity);
            var prefNet = pref.GetComponent<NetworkObject>();
            prefNet.Spawn();
        }
    }



    void OneAndOnlySave(string objName)
    {
        if (File.Exists(Application.persistentDataPath
          + "/DDGeneralII.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
              File.Open(Application.persistentDataPath
              + "/DDGeneralII.dat", FileMode.Open);
            SaveItems data = (SaveItems)bf.Deserialize(file);
            if (data.savesOneAndOnlyNames.Contains(objName + "SpawnPos")) return;
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
            OAOList.Add(objName + "SpawnPos");
            data.savesOneAndOnlyNames = OAOList;
            Debug.Log(data.savesOneAndOnlyNames.Count);
            bf.Serialize(file, data);
            file.Close();
            Debug.Log("OAOObject data saved!");
        }
    }

    [ServerRpc]
    void OneAndOnlySaveServerRpc(string objName)
    {
        if (File.Exists(Application.persistentDataPath
          + "/DDGeneralII.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
              File.Open(Application.persistentDataPath
              + "/DDGeneralII.dat", FileMode.Open);
            SaveItems data = (SaveItems)bf.Deserialize(file);
            if (data.savesOneAndOnlyNames.Contains(objName + "SpawnPos")) return;
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
            OAOList.Add(objName + "SpawnPos");
            data.savesOneAndOnlyNames = OAOList;
            Debug.Log(data.savesOneAndOnlyNames.Count);
            bf.Serialize(file, data);
            file.Close();
            Debug.Log("OAOObject data saved!");
        }
    }
}