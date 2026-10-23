using System;
using UnityEngine;
using Unity.Netcode;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UIElements;

[Serializable]
public class ItemInfo : NetworkBehaviour
{
    public Item item;
    public bool grabable = false;

    void Start()
    {
        Physics.IgnoreLayerCollision(3, 6, true);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.SetParent(collision.transform, true);
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().isKinematic = true;
            grabable = true;
        }
    }
}

[Serializable]

public class Item
{
    [SerializeField] public int id;
    [Header("Visual")]
    [SerializeField] public Sprite imageOnCanvas;
    [SerializeField] public Mesh model;
    [SerializeField] public Material[] materials;
    [SerializeField] public GameObject disappearEffect;
    [Header("Audio")]
    [SerializeField] public AudioClip pick;
    [SerializeField] public AudioClip drop;
    [Header("Stats")]
    [SerializeField] public Enums.ItemType type; //аппендикс
    [SerializeField] public Enums.UsableItemType usableType;
    [SerializeField] public KeyCode activateButton;
    [SerializeField] public KeyCode useButton;
    [SerializeField] public float energy;
    [SerializeField] public float weight;
    [SerializeField] public bool oneAndOnly;

    public void CopyItemInfo(Item item)
    {
        id = item.id;
        imageOnCanvas = item.imageOnCanvas;
        model = item.model;
        materials = item.materials;
        disappearEffect = item.disappearEffect;
        pick = item.pick;
        drop = item.drop;
        type = item.type;
        usableType = item.usableType;
        activateButton = item.activateButton;
        useButton = item.useButton;
        energy = item.energy;
        weight = item.weight;
        oneAndOnly = item.oneAndOnly;
    }

    public void NullItemInfo()
    {
        id = 0;
        imageOnCanvas = null;
        model = null;
        materials = null;
        disappearEffect = null;
        pick = null;
        drop = null;
        type = Enums.ItemType.None;
        usableType = Enums.UsableItemType.None;
        activateButton = KeyCode.None;
        useButton = KeyCode.None;
        energy = 0;
        weight = 0;
        oneAndOnly = false;
    }
}