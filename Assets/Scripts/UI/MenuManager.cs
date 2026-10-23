using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject otherMenu;

    public void MenuOpen()
    {
        menu.SetActive(true);
        if (otherMenu != null) otherMenu.SetActive(false);
    }

    public void MenuClose()
    {
        menu.SetActive(false);
        if (otherMenu != null) otherMenu.SetActive(true);
    }
}
