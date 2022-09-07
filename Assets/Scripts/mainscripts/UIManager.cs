using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("EVENTS")]
    [SerializeField] GameEvent OnShootEvent;
    [SerializeField] GameEvent OnHitEvent;
    [SerializeField] GameEvent OnCanHideEvent;
    [SerializeField] GameEvent OnCanInstaPunchEvent;


    [Header("PLAYER STATUS")]
    [SerializeField] Image healthBar;
    [SerializeField] TMP_Text healthLabel;
    [SerializeField] TMP_Text ammoLabel;

    [Header("TOOLTIP")]
    [SerializeField] GameObject hideTooltip;
    [SerializeField] GameObject punchTooltip;

    private void OnEnable()
    {
        OnShootEvent.AddListener<FloatEvent>(OnShootListener);
        OnHitEvent.AddListener<FloatEvent>(OnHitListener);
        OnCanHideEvent.AddListener<BoolEvent>(OnCanHideListener);
        OnCanInstaPunchEvent.AddListener<FloatEvent>(OnCanPunchListener);
    }
    private void OnDisable()
    {
        OnShootEvent.RemoveListener<FloatEvent>(OnShootListener);
        OnHitEvent.RemoveListener<FloatEvent>(OnHitListener);
        OnCanHideEvent.RemoveListener<BoolEvent>(OnCanHideListener);
        OnCanInstaPunchEvent.RemoveListener<FloatEvent>(OnCanPunchListener);
    }

    private void OnShootListener(FloatEvent f)
    {
        if (f.value.Length < 2)
            return; // Invalid length

        // Update ammo UI
        ammoLabel.text = $"{f.value[0]}/{f.value[1]}";
    }

    private void OnHitListener(FloatEvent f)
    {
        if (f.value.Length < 2)
            return; // Invalid length

        // Update health UI
        float curLife = f.value[0];
        float maxLife = f.value[1];

        healthBar.fillAmount = curLife / maxLife;
        healthLabel.text = $"{curLife}/{maxLife}";
    }

    private void OnCanHideListener(BoolEvent b)
    {
        hideTooltip.SetActive(b.state);
    }

    private void OnCanPunchListener(FloatEvent f)
    {
        punchTooltip.SetActive(f.value[0] > 0);
    }
}
