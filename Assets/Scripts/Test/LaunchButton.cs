using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchButton : MonoBehaviour
{
    [SerializeField] private Animator ship;

    void OnMouseDown()
    {
        ship.SetBool("fly", true);
    }
}
