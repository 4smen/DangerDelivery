using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    [Header("Current Gun Type")]
    public Enums.GunType gunType;

    [Header("Gun Settings")]
    [SerializeField] private List<float> GunDamage = new List<float>();

    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform dropPoint;

    [SerializeField] private float distance;
    [SerializeField] private float impactForce;
    [SerializeField] private float timeToNextShoot;

    [SerializeField] public bool showed;

    bool waitingToShoot;
    float waiting;

    [Header("Prefabs")]
    [SerializeField] private GameObject glockPrefab;

    [Header("Links")]
    [SerializeField] private Transform cam;

    [SerializeField] private GameObject helpPanel;
    [SerializeField] private Animator helpPanelText;

    [SerializeField] private TMP_InputField impactForceInput;
    [SerializeField] private TMP_InputField timeToShootInput;
    [SerializeField] private TMP_InputField distanceInput;

    GravityGun gravityGun;
    Animator animator;
    Controls controls;

    void Shoot()
    {
        Debug.Log("инициализировано");
        RaycastHit hit;
        Debug.DrawRay(shootPoint.position, shootPoint.forward * distance);
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, distance))
        {
            Debug.Log("луч попал");
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("попал в игрока");
                HealthBar player = hit.collider.gameObject.GetComponent<HealthBar>();
                //if player != owner
                if (player.immortality == false)
                {
                    Debug.Log("не бессмертный");
                    //player.HP -= GunDamage[(int)gunType];
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(cam.forward * impactForce);
                        Debug.Log("папали!");
                    }
                }
            }
            else
            {
                Debug.Log("не игрох к сожалению :(");
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(cam.forward * impactForce);
                    Debug.Log("папали!");
                }
                else
                {
                    Debug.Log("Фихня какая та :(");
                }
            }
        }
    }

    void DropGun()
    {
        HelpPanelShowDown();

        switch (gunType)
        {
            case Enums.GunType.Glock:
                var pref = Instantiate(glockPrefab);
                pref.transform.position = dropPoint.position;
                pref.transform.rotation = Quaternion.identity;
                var bigPrefNetObj = pref.GetComponent<NetworkObject>();
                bigPrefNetObj.Spawn();
                break;
        }

        gunType = Enums.GunType.None;
        animator.SetInteger("showedGun", 0);
        showed = false;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        gravityGun = GetComponent<GravityGun>();
        controls = GetComponent<Controls>();

        waiting = timeToNextShoot;

        impactForceInput.text = impactForce.ToString();
        timeToShootInput.text = timeToNextShoot.ToString();
        distanceInput.text = distance.ToString();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (waitingToShoot == true)
        {
            waiting -= Time.deltaTime;
            if (waiting <= 0)
            {
                waitingToShoot = false;
                waiting = timeToNextShoot;
            }
        }

        if (gunType != Enums.GunType.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (showed == true && waitingToShoot == false)
                {
                    Debug.Log("прожата кнопка");
                    waitingToShoot = true;
                    Shoot();
                }
            }

            if (Input.GetKeyDown(controls.showGunKey) && showed == false)
            {
                animator.SetInteger("showedGun", (int)gunType);
                showed = true;
                //gravityGun.ItemDrop();
            }
            else if (Input.GetKeyDown(controls.showGunKey) && showed == true)
            {
                animator.SetInteger("showedGun", 0);
                showed = false;
            }

            if (Input.GetKeyDown(controls.dropKey))
            {
                DropGun();
            }
        }
    }

    public void HelpPanelShowUp()
    {
        if (!IsOwner) return;

        helpPanel.SetActive(true);
        helpPanelText.SetBool("opened", true);
        StartCoroutine(HelpCloseUpTimer());
    }

    IEnumerator HelpCloseUpTimer()
    {
        yield return new WaitForSeconds(12f);
        helpPanelText.SetBool("opened", false);
    }

    public void HelpPanelShowDown()
    {
        helpPanel.SetActive(false);
    }



    public void ImpactForceChanged()
    {
        float inputedForce = System.Convert.ToSingle(impactForceInput.text);
        impactForce = inputedForce;
    }

    public void TimeToShootChanged()
    {
        float inputedTime = System.Convert.ToSingle(timeToShootInput.text);
        timeToNextShoot = inputedTime;
    }

    public void DistanceChanged()
    {
        float inputedDistance = System.Convert.ToSingle(distanceInput.text);
        distance = inputedDistance;
    }
}
