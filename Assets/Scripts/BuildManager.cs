using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private Turret[] turrets; // drag your turret prefabs here

    private int selectedTurret = 0;

    private void Awake()
    {
        main = this;
    }

    public Turret GetSelectedTurret()
    {
        return turrets[selectedTurret];
    }

    public void SetSelectedTurret(int _selectedTurret)
    {
        selectedTurret = _selectedTurret;
    }
}
