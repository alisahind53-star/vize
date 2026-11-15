using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Para")]
    [SerializeField] private int money = 100;

    [Header("Enerji (%)")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = 100f;

    [Header("Mutluluk (%)")]
    [SerializeField] private float maxHappiness = 100f;
    [SerializeField] private float currentHappiness = 100f;

    // UI ve diğer sistemlerin dinleyebileceği olaylar (Observer Pattern)
    public event Action<int> OnMoneyChanged;
    public event Action<float> OnEnergyChanged;    // 0.0 - 1.0 arası oran gönderir
    public event Action<float> OnHappinessChanged; // 0.0 - 1.0 arası oran gönderir

    public int Money => money;
    public float Energy => currentEnergy;
    public float MaxEnergy => maxEnergy;
    public float EnergyNormalized => maxEnergy > 0f ? currentEnergy / maxEnergy : 0f;

    public float Happiness => currentHappiness;
    public float MaxHappiness => maxHappiness;
    public float HappinessNormalized => maxHappiness > 0f ? currentHappiness / maxHappiness : 0f;

    private void Start()
    {
        // Başlangıç değerlerini UI'a bildir
        OnMoneyChanged?.Invoke(money);
        OnEnergyChanged?.Invoke(EnergyNormalized);
        OnHappinessChanged?.Invoke(HappinessNormalized);
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0 || money < amount) return false;
        money -= amount;
        OnMoneyChanged?.Invoke(money);
        return true;
    }

    public void ChangeEnergy(float delta)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + delta, 0f, maxEnergy);
        OnEnergyChanged?.Invoke(EnergyNormalized);
    }

    public void SetEnergy(float value)
    {
        currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
        OnEnergyChanged?.Invoke(EnergyNormalized);
    }

    public void ChangeHappiness(float delta)
    {
        currentHappiness = Mathf.Clamp(currentHappiness + delta, 0f, maxHappiness);
        OnHappinessChanged?.Invoke(HappinessNormalized);
    }

    public void SetHappiness(float value)
    {
        currentHappiness = Mathf.Clamp(value, 0f, maxHappiness);
        OnHappinessChanged?.Invoke(HappinessNormalized);
    }
}