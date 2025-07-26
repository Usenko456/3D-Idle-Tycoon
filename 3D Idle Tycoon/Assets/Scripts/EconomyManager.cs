using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }
    
    public float Balance { get; private set; } = 5000f;

    public float roadCost = 30f;
    public float houseCost = 200f;
    public float specialCost = 300f;

    public float roadIncome = 1f;      
    public float houseIncome = 5f;
    public float specialIncome = 10f;

    private float incomeTimer = 0f;
    private float incomeInterval = 60f; 

    private List<Building> buildings = new List<Building>();

    public float CurrentIncomePerMinute { get; private set; } = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        RecalculateIncomePerMinute(); 
    }

    private void Update()
    {
        incomeTimer += Time.deltaTime;
        if (incomeTimer >= incomeInterval)
        {
            ApplyIncome();
            incomeTimer = 0f;
        }
    }

    private void ApplyIncome()
    {
        float income = CurrentIncomePerMinute;
        Balance += income;

        Debug.Log($"+{income}$ нараховано прибутку за {incomeInterval} секунд. Баланс: {Balance}$");
    }

    public bool CanAfford(float cost) => Balance >= cost;

    public bool TryBuild(BuildingType type, Vector2 gridPos, bool isLoading)
    {
        float cost = type switch
        {
            BuildingType.Road => roadCost,
            BuildingType.House => houseCost,
            BuildingType.Special => specialCost,
            BuildingType.BigHause=>houseCost,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!isLoading && !CanAfford(cost))
        {
            Debug.LogWarning($"Недостатньо грошей для побудови дороги на ");
            return false;
        }

            

        if (!isLoading)
            Balance -= cost;

        buildings.Add(new Building(type, gridPos, GetIncomeForType(type)));
        RecalculateIncomePerMinute();

        Debug.Log($"Побудовано {type} на {gridPos}, списано: {(isLoading ? 0 : cost)}$. Баланс: {Balance}$");
        return true;
    }


    public void RecalculateIncomePerMinute()
    {
        float totalIncomePerMinute = 0f;
        foreach (var b in buildings)
        {
            totalIncomePerMinute += b.GetIncomePerMinute();
        }

        CurrentIncomePerMinute = totalIncomePerMinute;
    }

    private float GetIncomeForType(BuildingType type) => type switch
    {
        BuildingType.Road => roadIncome,
        BuildingType.House => houseIncome,
        BuildingType.Special => specialIncome,
        BuildingType.BigHause=>houseIncome,
        _ => 0f
    };

    public class Building
    {
        public BuildingType type;
        public Vector2 gridPosition;
        private float incomePerMinute;

        public Building(BuildingType type, Vector2 pos, float income)
        {
            this.type = type;
            gridPosition = pos;
            incomePerMinute = income;
        }

        public float GetIncomePerMinute() => incomePerMinute;
    }
    public void SetBalance(float value)
    {
        Balance = value;
        Debug.Log($"Баланс встановлено: {Balance}");
    }
    public enum BuildingType
    {
        Empty,
        Road,
        House,
        Special,
        BigHause
    }
}
