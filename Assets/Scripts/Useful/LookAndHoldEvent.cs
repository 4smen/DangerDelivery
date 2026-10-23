using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class LookAndHoldEvent : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image scale;
    bool showed = false;
    float countdown = 0f;

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Functional") && showed == false)
            {
                window.SetActive(true);
                showed = true;
                switch (hit.collider.gameObject.name)
                {
                    case "Door":
                        text.text = $"Open: {Controls.instance.interactKey}";
                        break;
                    case "ShipLever":
                        text.text = $"Start Ship: {Controls.instance.interactKey}";
                        break;
                }
            }
            else if (!hit.collider.CompareTag("Functional") && showed == true)
            {
                window.SetActive(false);
                showed = false;
            }
        }
        if (Input.GetKey(Controls.instance.interactKey) && showed == true)
        {
            countdown += Time.deltaTime * .4f;
            scale.fillAmount = countdown / 1;
            if (countdown >= 1)
            {              
                countdown = 0;
                switch (hit.collider.gameObject.name)
                {
                    case "Door":
                        //hit.collider.gameObject.GetComponent<Animator>();
                        break;
                    //case "ShipLever":
                        //if (hit.collider.gameObject.GetComponentInParent<ShipManager>().TryShipStarting())
                        //{
                        //    //hit.collider.gameObject.GetComponent<Animator>();
                        //}
                        //break;
                }
                showed = false;
            }
        }
        if (Input.GetKeyUp(Controls.instance.interactKey))
        {
            countdown -= Time.deltaTime * .4f;
            scale.fillAmount = countdown / 1;
        }
    }
}
