using Unity.Netcode;
using UnityEngine;

public class MenuOpener : NetworkBehaviour
{
    [Header("General Menus")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("Menus")]
    [SerializeField] private GameObject debugMenu;
    [SerializeField] private GameObject videoMenu;
    [SerializeField] private GameObject audioMenu;

    [Header("Links")]
    [SerializeField] private FirstPersonController fps;
    [SerializeField] private GravityGun gravityGun;
    [SerializeField] private Inventory inventory;
    //[SerializeField] private Gun gun;
    Controls controls;

    [Header("Booleans")]
    [SerializeField] private bool opened;

    public void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            if (!IsOwner) { canvas.SetActive(false); return; }
        }
    }

    void Start()
    {
        controls = GetComponent<Controls>();

        canvas.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        debugMenu.SetActive(false);
        videoMenu.SetActive(false);
        audioMenu.SetActive(false);
        opened = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(controls.pauseKey))
        {          
            if (opened == false) MenuOpen();
            else if (opened == true) MenuClose();
        }
    }

    public void MenuOpen()
    {
        if (!IsOwner) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fps.enabled = false;
        gravityGun.enabled = false;
        inventory.enabled = false;
        //gun.enabled = false;

        opened = !opened;
        pauseMenu.SetActive(true);
    }
    public void MenuClose()
    {
        if (!IsOwner) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fps.enabled = true;
        gravityGun.enabled = true;
        inventory.enabled = true;
        //gun.enabled = true;

        opened = !opened;
        pauseMenu.SetActive(false);
    }


    public void SettingsOpen()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void SettingsClose()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void DebugMenuOpen()
    {
        debugMenu.SetActive(true);
    }
    public void DebugMenuClose()
    {
        debugMenu.SetActive(false);
    }

    public void VideoMenuOpen()
    {
        videoMenu.SetActive(true);
    }
    public void VideoMenuClose()
    {
        videoMenu.SetActive(false);
    }

    public void AudioMenuOpen()
    {
        audioMenu.SetActive(true);
    }
    public void AudioMenuClose()
    {
        audioMenu.SetActive(false);
    }
}
