﻿using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private TowerBase[] towers; // Assign all tower prefabs in the Inspector

    private int selectedTowerIndex = 0;
    private Dictionary<int, int> towerCounts = new Dictionary<int, int>();

    public static List<GameObject> PlacedTowers { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        main = this;

        // Initialize counts
        for (int i = 0; i < towers.Length; i++)
            towerCounts[i] = 0;
    }

    // ==============================
    // TOWER SELECTION
    // ==============================
    public TowerBase GetSelectedTower() => towers[selectedTowerIndex];
    public int GetSelectedIndex() => selectedTowerIndex;
    public void SetSelectedTower(int index) => selectedTowerIndex = index;

    // ==============================
    // BUILD TRACKING
    // ==============================
    public void RegisterBuiltTower(int index)
    {
        if (towerCounts.ContainsKey(index))
            towerCounts[index]++;
    }

    public int GetTowerCount(int index)
    {
        return towerCounts.ContainsKey(index) ? towerCounts[index] : 0;
    }

    public Dictionary<int, int> GetAllTowerCounts() => towerCounts;

    public void RegisterTowerInstance(GameObject tower)
    {
        if (tower != null && !PlacedTowers.Contains(tower))
            PlacedTowers.Add(tower);
    }

    public void ClearAllTowers()
    {
        PlacedTowers.Clear();
        for (int i = 0; i < towers.Length; i++)
            towerCounts[i] = 0;
    }

    public TowerBase[] GetTowers() => towers;
}