using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float maxHitPoints;
    private float currentHitPoints;

    [SerializeField] private HealthBar healthBar;

    private void Awake()
    {
        if (healthBar == null) 
            healthBar = GetComponentInChildren<HealthBar>();
    }

    // Other scripts can subscribe to this (e.g. Slime)
    public event Action OnDeath;

    // Called by EnemyStats when the enemy spawns
    public void InitializeHealth(float newMaxHP)
    {
        maxHitPoints = Mathf.Max(1, newMaxHP);
        currentHitPoints = maxHitPoints;
        UpdateBarFull();
    }

    private void UpdateBarFull()
    {
        if (healthBar != null)
            healthBar.SetCurrentHealth(currentHitPoints);
    }

    /// <summary>
    /// Deals float damage — supports decimal amounts like 0.1f, 0.25f, etc.
    /// Handles shields as an extra HP layer (no overflow to HP if shield breaks).
    /// </summary>
    public void TakeDamage(float dmg)
    {
        // --- Check for active shield ---
        Shield shield = GetComponent<Shield>();
        if (shield != null && shield.IsActive())
        {
            shield.TakeShieldDamage(dmg);
            return;
        }

        // --- Apply HP damage ---
        currentHitPoints -= dmg;
        currentHitPoints = Mathf.Max(currentHitPoints, 0f);

        if (healthBar != null) 
            healthBar.SetCurrentHealth(currentHitPoints);

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
        EnemySpawner.onEnemyDestroy?.Invoke();
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
    }
}


