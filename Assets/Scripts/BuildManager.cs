using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
<<<<<<< HEAD
    [SerializeField] private Turret[] turrets; // assign in Inspector

    private int selectedTurret = -1; // start with nothing selected
=======
    [SerializeField] private TowerBase[] towers; // Assign all tower prefabs in the Inspector

    private int selectedTowerIndex = 0;
    private Dictionary<int, int> towerCounts = new Dictionary<int, int>();

    public static List<GameObject> PlacedTowers { get; private set; } = new List<GameObject>();
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f

    private void Awake()
    {
        main = this;

<<<<<<< HEAD
        if (turrets != null && turrets.Length > 0 && turrets[0] != null){
            SetSelectedTurret(0); // default to first turret;
        }
=======
        // Initialize counts
        for (int i = 0; i < towers.Length; i++)
            towerCounts[i] = 0;
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f
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
<<<<<<< HEAD
        if (turrets == null || turrets.Length == 0)
        {
            Debug.LogError("[BuildManager] No turret prefabs assigned.");
            return null;
        }
        if (selectedTurret < 0 || selectedTurret >= turrets.Length)
        {
            Debug.LogWarning($"[BuildManager] Invalid selection index {selectedTurret}. Valid: 0..{turrets.Length - 1}");
            return null;
        }
        var t = turrets[selectedTurret];
        if (t == null)
        {
            Debug.LogError($"[BuildManager] Turret at index {selectedTurret} is NULL.");
            return null;
        }
        return t;
    }

    public void SetSelectedTurret(int index)
    {
        if (turrets == null || turrets.Length == 0)
        {
            Debug.LogError("[BuildManager] Cannot select—no turrets assigned.");
            selectedTurret = -1;
            return;
        }
        if (index < 0 || index >= turrets.Length)
        {
            Debug.LogError($"[BuildManager] Index {index} out of range 0..{turrets.Length - 1}.");
            selectedTurret = -1;
            return;
        }
        selectedTurret = index;
    }

    public void ClearSelection() => selectedTurret = -1;

    public bool HasSelection => turrets != null && selectedTurret >= 0 && selectedTurret < turrets.Length;
=======
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
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f
}

