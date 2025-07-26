using UnityEngine;
using TMPro; 

public class EconomyUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text incomeText;

    private EconomyManager economyManager;

    private void Start()
    {
        economyManager = FindObjectOfType<EconomyManager>();
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI(); 
    }

    public void UpdateUI()
    {
        if (economyManager == null) return;
        balanceText.text = $"Balance: ${economyManager.Balance}";
        incomeText.text = $"Income (per min): ${economyManager.CurrentIncomePerMinute}";
    }
}
