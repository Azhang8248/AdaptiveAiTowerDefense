using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private Turret[] turrets; // assign in Inspector

    private int selectedTurret = -1; // start with nothing selected

    private void Awake()
    {
        main = this;

        if (turrets != null && turrets.Length > 0 && turrets[0] != null){
            SetSelectedTurret(0); // default to first turret;
        }
    }

    public Turret GetSelectedTurret()
    {
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
}

