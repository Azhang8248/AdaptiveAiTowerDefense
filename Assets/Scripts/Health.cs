using UnityEngine;

public class Health : MonoBehaviour
{
    private int maxHitPoints;
    private int currentHitPoints;

    [SerializeField] private HealthBar healthBar;

    // Called by EnemyStats when the enemy spawns
    public void InitializeHealth(int newMaxHP)
    {
        maxHitPoints = newMaxHP;
        currentHitPoints = maxHitPoints;

        if (healthBar != null)
            healthBar.UpdateBar(maxHitPoints, currentHitPoints);
    }

    public void TakeDamage(int dmg)
    {
        currentHitPoints -= dmg;

        if (healthBar != null)
            healthBar.SetCurrentHealth(currentHitPoints);

        if (currentHitPoints <= 0)
            Die();
    }

    private void Die()
    {
        // Notify the spawner (so it knows the enemy died)
        EnemySpawner.onEnemyDestroy.Invoke();

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
