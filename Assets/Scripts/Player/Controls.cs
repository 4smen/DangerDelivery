using UnityEngine;

public class Controls : MonoBehaviour
{
    public KeyCode crouchKey;
    public KeyCode sprintKey;

    public KeyCode interactKey;
    public KeyCode collectKey;

    public KeyCode dropKey;

    public KeyCode showGunKey;
    public KeyCode reloadKey;

    public KeyCode pauseKey;
    public KeyCode cancelKey;

    public KeyCode handTap;

    public static Controls instance;

    void Awake()
    {
        instance = this;
    }
}
