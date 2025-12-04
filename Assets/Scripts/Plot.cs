using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

<<<<<<< HEAD
    private GameObject turretObj;
    public Turret pturret;
=======
    private GameObject tower;
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;

        // ✅ Make plot 25% visible (75% transparent)
        Color c = sr.color;
        c.a = .1f;
        sr.color = c;
        startColor = c; // so hover reset returns to the same transparency
    }

    private void OnMouseEnter()
    {
        // 🔒 Disable hover when shop open
        if (FindFirstObjectByType<ShopManager>()?.IsOpen == true)
            return;

        // Preserve transparency when hovered
        Color hc = hoverColor;
        hc.a = 1f;
        sr.color = hc;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
<<<<<<< HEAD
        if (UIManager.main.IsHoveringUI()) return;

        if (turretObj != null)
        {
            pturret.OpenUpgradeUI();
            return;
        } 
=======
        // 🔒 Ignore clicks when shop open
        if (FindFirstObjectByType<ShopManager>()?.IsOpen == true)
            return;
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f

        // Don’t allow placing multiple towers on the same plot
        if (tower != null) return;

        TowerBase towerToBuild = BuildManager.main.GetSelectedTower();
        if (towerToBuild == null)
        {
            Debug.LogWarning("No tower selected!");
            return;
        }

        // Check if the player can afford it
        if (towerToBuildPrice(towerToBuild) > LevelManager.main.playerGold)
        {
            Debug.Log("You cannot afford this tower!");
            return;
        }

<<<<<<< HEAD
        // Spend gold and build the turret
        LevelManager.main.SpendGold(turretToBuild.GetPrice());
        turretObj = Instantiate(turretToBuild.gameObject, transform.position, Quaternion.identity);
        pturret = turretObj.GetComponent<Turret>();
=======
        // Deduct gold and build the tower
        LevelManager.main.SpendGold(towerToBuildPrice(towerToBuild));
        tower = Instantiate(towerToBuild.gameObject, transform.position, Quaternion.identity);

        // Register build event
        BuildManager.main.RegisterBuiltTower(BuildManager.main.GetSelectedIndex());
        BuildManager.main.RegisterTowerInstance(tower);
    }

    private int towerToBuildPrice(TowerBase tower)
    {
        var field = tower?.GetType().GetField("price", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (int)field.GetValue(tower) : 0;
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f
    }
}
