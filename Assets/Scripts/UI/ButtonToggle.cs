using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public GameObject button;
    Color buttonColor;
    public Color pressedColor;
    bool pressed;

    void Start()
    {
        pressed = false;
        buttonColor = button.GetComponent<Image>().color;
    }

    public void Pressed()
    {
        button.GetComponent<Image>().color = pressedColor;
        pressed = !pressed;
    }

    public void Unpressed()
    {
        button.GetComponent<Image>().color = buttonColor;
        pressed = !pressed;
    }
}
