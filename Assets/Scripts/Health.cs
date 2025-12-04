using System;
using UnityEngine;

public class Health : MonoBehaviour
{
<<<<<<< HEAD
    [Header("Attributes")]
    [SerializeField] private int maxHitPoints = 2;   // can be overridden by EnemyStats.InitializeHealth
    [SerializeField] private int dollarAmount = 100;

    [Header("UI")]
    [SerializeField] private HealthBar healthBar;    // assign in Inspector OR auto-find in Awake

    private int currentHitPoints;
    private bool isDestroyed = false;
=======
    private float maxHitPoints;
    private float currentHitPoints;
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f

    private void Awake()
    {
        if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>();

<<<<<<< HEAD
        currentHitPoints = Mathf.Max(1, maxHitPoints);
        UpdateBarFull();
    }

    /// <summary>Called by EnemyStats to set HP per enemy.</summary>
    public void InitializeHealth(int newMaxHP)
=======
    // Other scripts can subscribe to this (e.g. Slime)
    public event Action OnDeath;

    // Called by EnemyStats when the enemy spawns
    public void InitializeHealth(float newMaxHP)
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f
    {
        maxHitPoints = Mathf.Max(1, newMaxHP);
        currentHitPoints = maxHitPoints;
        UpdateBarFull();
    }

    /// <summary>
    /// Deals float damage — supports decimal amounts like 0.1f, 0.25f, etc.
    /// Handles shields as an extra HP layer (no overflow to HP if shield breaks).
    /// </summary>
    public void TakeDamage(float dmg)
    {
<<<<<<< HEAD
        if (isDestroyed) return;
=======
        // --- Check for active shield ---
        Shield shield = GetComponent<Shield>();
        if (shield != null && shield.IsActive())
        {
            shield.TakeShieldDamage(dmg);
            return; // Shield absorbed the hit; don't affect health yet.
        }

        // --- Apply HP damage ---
        currentHitPoints -= dmg;
        currentHitPoints = Mathf.Max(currentHitPoints, 0f);
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f

        currentHitPoints = Mathf.Max(0, currentHitPoints - Mathf.Max(0, dmg));
        if (healthBar != null) healthBar.SetCurrentHealth(currentHitPoints);

<<<<<<< HEAD
        if (currentHitPoints <= 0)
        {
            isDestroyed = true;

            // ðŸ’° Award money when enemy dies
            LevelManager.main.AddGold(dollarAmount);

            EnemySpawner.onEnemyDestroy.Invoke();
            Destroy(gameObject);
        }
    }


    private void UpdateBarFull()
    {
        if (healthBar != null) healthBar.UpdateBar(maxHitPoints, currentHitPoints);
=======
        // --- Handle death ---
        if (currentHitPoints <= 0f)
            Die();
    }

    public float GetCurrentHitPoints() => currentHitPoints;
    public float GetMaxHitPoints() => maxHitPoints;
    public void SetMaxHitPoints(float hitPoints) => maxHitPoints = hitPoints;

    private void Die()
    {
        // Notify spawner that an enemy died
        EnemySpawner.onEnemyDestroy.Invoke();
        OnDeath?.Invoke();

        // Grant player gold based on enemy price
        EnemyStats stats = GetComponent<EnemyStats>();
        if (stats != null)
        {
            int reward = stats.price * 10;
            LevelManager.main.AddGold(reward);
            Debug.Log($"Enemy defeated! +{reward} gold (Total: {LevelManager.main.playerGold})");
        }

        // Destroy the enemy object
        Destroy(gameObject);
>>>>>>> a4349a900022c5284c281b5d7e7df56ed328550f
    }
}

