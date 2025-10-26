using System;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int maxHitPoints; // set per prefab
    private int currentHitPoints;
    [SerializeField] public HealthBar healthBar;

    // Other scripts can subscribe to this (slime)
    public event Action OnDeath;

    // Called by EnemyStats when the enemy spawns
    public void InitializeHealth(int newMaxHP)
    {
        currentHitPoints = maxHitPoints; // initialize from prefab
        healthBar.UpdateBar(maxHitPoints, currentHitPoints);
    }

    public void TakeDamage(int dmg)
    {
        currentHitPoints -= dmg;
        healthBar.SetCurrentHealth(currentHitPoints);
        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    public int getMaxHitPoints()
    {
        return maxHitPoints;
    }
   
   public void setMaxHitPoints(int hitPoints)
   {
        maxHitPoints = hitPoints;
   }

    private void Die()
    {
        // Notify the spawner (so it knows the enemy died)
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

        // Destroy enemy
        Destroy(gameObject);
   }
}
