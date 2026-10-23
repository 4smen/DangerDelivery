using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDelete : MonoBehaviour
{
    public GameObject parent;
    void Awake()
    {
        var obj = GameObject.Find(gameObject.name);
        if (obj != parent && obj != null)
        {
            Destroy(parent);
        }
    }
}
