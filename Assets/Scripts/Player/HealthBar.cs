using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class HealthBar : MonoBehaviour
{
    public delegate void onHPChanged(float hp);
    public event onHPChanged HpChanged;

    [SerializeField] private TextMeshProUGUI healthCount;
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] public NetworkVariable<float> HP = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public bool immortality;
    Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
        HP.Value = maxHealth;

        HpChanged += OnHPCountChanged;
    }

    
    //void Update()
    //{
    //    healthBar.fillAmount = HP / maxHealth;
    //}

    void OnHPCountChanged(float newHp)
    {
        HP.Value += newHp;
        healthCount.text = newHp.ToString() + "%";
    }

    public void ImmortalityChanged()
    {
        immortality = !immortality;
    }
}
