using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] private Animator door;
    bool toggle = false;

    void OnMouseDown()
    {
        toggle = !toggle;
        door.SetBool("open", toggle);
    }
}
