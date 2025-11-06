using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int maxHitPoints = 2;   // can be overridden by EnemyStats.InitializeHealth
    [SerializeField] private int dollarAmount = 100;

    [Header("UI")]
    [SerializeField] private HealthBar healthBar;    // assign in Inspector OR auto-find in Awake

    private int currentHitPoints;
    private bool isDestroyed = false;

    private void Awake()
    {
        if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>();

        currentHitPoints = Mathf.Max(1, maxHitPoints);
        UpdateBarFull();
    }

    /// <summary>Called by EnemyStats to set HP per enemy.</summary>
    public void InitializeHealth(int newMaxHP)
    {
        maxHitPoints = Mathf.Max(1, newMaxHP);
        currentHitPoints = maxHitPoints;
        UpdateBarFull();
    }

    public void TakeDamage(int dmg)
    {
        if (isDestroyed) return;

        currentHitPoints = Mathf.Max(0, currentHitPoints - Mathf.Max(0, dmg));
        if (healthBar != null) healthBar.SetCurrentHealth(currentHitPoints);

        if (currentHitPoints <= 0)
        {
            isDestroyed = true;

            // 💰 Award money when enemy dies
            LevelManager.main.AddGold(dollarAmount);

            EnemySpawner.onEnemyDestroy.Invoke();
            Destroy(gameObject);
        }
    }


    private void UpdateBarFull()
    {
        if (healthBar != null) healthBar.UpdateBar(maxHitPoints, currentHitPoints);
    }
}

