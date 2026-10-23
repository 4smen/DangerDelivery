using UnityEngine;
using Unity.Netcode;

public class HeadRotate : NetworkBehaviour
{
    public Transform cam;
    public Transform head;
    public bool isHand;
    public bool showed;

    void LateUpdate()
    {
        if (showed == true)
        {
            if (!IsOwner) return;
            Rotate();
        }
        if (isHand == true)
        {
            if (Input.GetKey(Controls.instance.handTap))
            {
                if (!IsOwner) return;
                Rotate();
            }
        }
        else
        {
            if (!IsOwner) return;
            Rotate();
        }
    }

    void Rotate()
    {
        head.rotation = cam.rotation;
    }
}
