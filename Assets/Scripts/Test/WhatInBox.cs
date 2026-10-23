using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhatInBox : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        Debug.Log("В " + gameObject.name + " попала фигня под названием " + other.name);
    }
}
