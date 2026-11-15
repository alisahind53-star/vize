using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Para Göstergesi")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private string currencySymbol = "₺";

    [Header("Enerji Barı (Image Type: Filled)")]
    [SerializeField] private Image energyFillImage;
    [SerializeField] private TextMeshProUGUI energyPercentText;

    [Header("Mutluluk Barı (Image Type: Filled)")]
    [SerializeField] private Image happinessFillImage;
    [SerializeField] private TextMeshProUGUI happinessPercentText;

    [Header("Animasyon Ayarı")]
    [Tooltip("Barların yumuşak dolum hızı. 0 yaparsanız anında güncellenir.")]
    [SerializeField] private float smoothSpeed = 8f;

    private float targetEnergyFill = 1f;
    private float targetHappinessFill = 1f;

    private void OnEnable()
    {
        if (playerStats == null) return;

        playerStats.OnMoneyChanged += UpdateMoneyUI;
        playerStats.OnEnergyChanged += UpdateEnergyUI;
        playerStats.OnHappinessChanged += UpdateHappinessUI;
    }

    private void OnDisable()
    {
        if (playerStats == null) return;

        playerStats.OnMoneyChanged -= UpdateMoneyUI;
        playerStats.OnEnergyChanged -= UpdateEnergyUI;
        playerStats.OnHappinessChanged -= UpdateHappinessUI;
    }

    private void Start()
    {
        if (playerStats != null)
        {
            UpdateMoneyUI(playerStats.Money);
            UpdateEnergyUI(playerStats.EnergyNormalized);
            UpdateHappinessUI(playerStats.HappinessNormalized);

            if (energyFillImage != null) energyFillImage.fillAmount = targetEnergyFill;
            if (happinessFillImage != null) happinessFillImage.fillAmount = targetHappinessFill;
        }
    }

    private void Update()
    {
        if (smoothSpeed <= 0f) return;

        if (energyFillImage != null)
        {
            energyFillImage.fillAmount = Mathf.Lerp(energyFillImage.fillAmount, targetEnergyFill, Time.deltaTime * smoothSpeed);
        }

        if (happinessFillImage != null)
        {
            happinessFillImage.fillAmount = Mathf.Lerp(happinessFillImage.fillAmount, targetHappinessFill, Time.deltaTime * smoothSpeed);
        }
    }

    private void UpdateMoneyUI(int newMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"{newMoney} {currencySymbol}".Trim();
        }
    }

    private void UpdateEnergyUI(float normalizedValue)
    {
        targetEnergyFill = Mathf.Clamp01(normalizedValue);

        if (smoothSpeed <= 0f && energyFillImage != null)
        {
            energyFillImage.fillAmount = targetEnergyFill;
        }

        if (energyPercentText != null)
        {
            energyPercentText.text = $"%{Mathf.RoundToInt(targetEnergyFill * 100f)}";
        }
    }

    private void UpdateHappinessUI(float normalizedValue)
    {
        targetHappinessFill = Mathf.Clamp01(normalizedValue);

        if (smoothSpeed <= 0f && happinessFillImage != null)
        {
            happinessFillImage.fillAmount = targetHappinessFill;
        }

        if (happinessPercentText != null)
        {
            happinessPercentText.text = $"%{Mathf.RoundToInt(targetHappinessFill * 100f)}";
        }
    }
}