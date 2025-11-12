using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int maxHitPoints;
    private int currentHitPoints;
    public HealthBar healthBar;
    public Animator animator;

    // Other scripts can subscribe to this (slime)
    public event Action OnDeath;

    // Called by EnemyStats when the enemy spawns

private void Awake()
   {
        animator = GetComponent<Animator>();
   }

    public void InitializeHealth(int newMaxHP)
    {
        maxHitPoints = newMaxHP;
        currentHitPoints = maxHitPoints;

        if (healthBar != null)
            healthBar.UpdateBar(maxHitPoints, currentHitPoints);
    }

    public void TakeDamage(int dmg)
    {
        // If shield exists, hit the shield
            var shield = GetComponent<Shield>();
            if (shield != null && shield.enabled)
            {
                shield.Absorb();
                return;
            }
            
            currentHitPoints -= dmg;

            if (healthBar != null)
                healthBar.SetCurrentHealth(currentHitPoints);

            if (currentHitPoints <= 0)
                Die();
        
    }

    public int getCurrentHitPoints()
   {
        return currentHitPoints;
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


    //     if(animator != null)
    //     {
    //     animator = GetComponent<Animator>();
    //   }

    //     animator.SetTrigger("Die");

        Destroy(gameObject);
   }
}
