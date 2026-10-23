using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    [SerializeField] private Enums.GunType gunTypePickUp;
    [SerializeField] private GameObject gun;
    public Animator textTable;

    void Awake()
    {
        var netObj = GetComponentInParent<NetworkObject>();
        netObj.Spawn();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textTable.SetBool("opened", true);
            if (Input.GetKeyDown(other.GetComponent<Controls>().collectKey))
            {
                if (other.GetComponent<Gun>().gunType == Enums.GunType.None)
                {
                    if (other.GetComponent<GravityGun>().holding == true)
                    {
                        //other.GetComponent<GravityGun>().ItemDrop();
                    }
                    other.GetComponent<Gun>().gunType = gunTypePickUp;
                    other.GetComponent<Gun>().HelpPanelShowUp();
                    Destroy(gun);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textTable.SetBool("opened", false);
        }
    }
}
